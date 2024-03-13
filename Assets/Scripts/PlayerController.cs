using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager;

public class PlayerController : MonoBehaviour
{
    public PlayerStats basePlayerStats; //I gave it a simple name because it appears EVERYWHERE in the code
    [HideInInspector] public List<PlayerStats> multiplicationList;
    [HideInInspector] public List<PlayerStats> additionList;
    [HideInInspector] public PlayerStats ps;

    //Inputs
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction dashAction;
    Vector2 moveValue;

    //Components
    public GameObject playerModel;
    Rigidbody rb;
    public GameObject camPivot; //Where the camera rotates around. Doesn't do much in first person but if we try third person it'll have an effect on it.
    public Camera cam;
    public LayerMask layerMask; //What objects the camera can move through, again only applies to third person
    //public AudioSource landSrc;
    //public AudioSource walkSrc;

    //Other variables
    float targetXRotation; //These two variables are used for camera rotation clamping purposes
    float targetYRotation;
    public bool onGround = false;
    public float camDist = 0f; //How far the camera is from the player. 0 = first person.
    public float groundDist = 0.8f; //how far from the ground does the player need to be to jump
    public bool hasControl = true;
    public float FOVChangeRate;



    public float walkIncrements = 0.8f;
    float walkTime = 0.5f;

    void Start()
    {
        ps = basePlayerStats;
        rb = GetComponent<Rigidbody>();
        targetXRotation = camPivot.transform.localRotation.x;
        targetYRotation = camPivot.transform.localRotation.y;
        Physics.gravity *= 4;
    }

    void Update()
    {
        Debug.Log(ps.dashTime);

        var onGroundLastFrame = onGround;
        onGround = Physics.BoxCast(playerModel.transform.position, new Vector3(1, 0.1f, 1) * 2, Vector3.down, Quaternion.identity, groundDist, layerMask);
        if(!onGroundLastFrame && onGround)
        {
            Land.Invoke(gameObject);
            //Play landing sound here
        }
        if (Time.timeScale > 0 && hasControl)
        {
            moveValue = moveAction.ReadValue<Vector2>();

            //Handle rotation
            targetXRotation -= Input.GetAxis("Mouse Y") * ps.rotateSpeed;
            targetYRotation += Input.GetAxis("Mouse X") * ps.rotateSpeed;
            targetXRotation = Mathf.Clamp(targetXRotation, -90, 90);

            camPivot.transform.eulerAngles = new Vector3(targetXRotation, camPivot.transform.eulerAngles.y, camPivot.transform.eulerAngles.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);
        }
        else
        {
            moveValue = new Vector2();
        }

        //Prevent camera from passing through walls
        RaycastHit hit;
        Ray ray = new Ray(camPivot.transform.position, cam.transform.position - camPivot.transform.position);
        Physics.Raycast(ray, out hit, camDist, layerMask);
        if (hit.collider == null)
        {
            cam.transform.position = camPivot.transform.position - camPivot.transform.forward * camDist;
        }
        else
        {
            cam.transform.position = hit.point + (-ray.direction.normalized * 0.3f);
        }

        //if(Mathf.Abs(new Vector2(rb.velocity.x, rb.velocity.z).magnitude) > 0.4 && onGround)
        //{
        //    walkTime -= Time.deltaTime;
        //    if(walkTime <= 0)
        //    {
        //        walkTime = walkIncrements;
        //        walkSrc.Play();
        //    }
        //}

        //Count down dash timers
        if (ps.dashTimeLeft > 0)
        {
            ps.dashTimeLeft -= Time.deltaTime;
            if(cam.fieldOfView < ps.dashFOV)
            {
                cam.fieldOfView += FOVChangeRate * Time.deltaTime;
            }
        }
        else
        {
            ps.maxMagnitude = ps.walkMaxMagnitude;
            ps.moveSpeed = ps.walkMoveSpeed;
            ps.deccelRate = ps.walkDeccelRate;
            if (cam.fieldOfView > ps.walkFOV)
            {
                cam.fieldOfView -= FOVChangeRate * Time.deltaTime;
            }
        }

        if (ps.dashCooldownTimeLeft > 0)
        {
            ps.dashCooldownTimeLeft -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (dashAction.IsPressed() && hasControl && ps.dashCooldownTimeLeft <= 0)
        {
            PlayerDash.Invoke(gameObject);
            ps.maxMagnitude = ps.dashMaxMagnitude;
            rb.AddForce(transform.forward * ps.dashForce);
            ps.dashTimeLeft = ps.dashTime;
            ps.dashCooldownTimeLeft = ps.dashCooldown;
            ps.moveSpeed = ps.dashMoveSpeed;
            ps.deccelRate = ps.dashDeccelRate;
        }

        if (jumpAction.IsPressed() && onGround && hasControl)
        {
            Jump.Invoke(gameObject);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(ps.jumpPower * Vector3.up);
        }
    }

    public void Movement()
    {
        //First, apply the force
        rb.AddRelativeForce(new Vector3(moveValue.x * ps.moveSpeed, 0, moveValue.y * ps.moveSpeed));

        if (moveValue.x == 0)
        {
            //This dampens x velocity when left and right aren't held. It first converts to local velocity, reduces it, then converts it back to global velocity.
            var localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity = new Vector3(localVelocity.x / ps.deccelRate, localVelocity.y, localVelocity.z);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
        if (moveValue.y == 0)
        {
            //This does the same but for z velocity
            var localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity = new Vector3(localVelocity.x, localVelocity.y, localVelocity.z / ps.deccelRate);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
        if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude > ps.maxMagnitude)
        {
            //Finally, it clamps the magnitude of the velocity so that the player doesn't accelerate to infinity
            var clampedVelocity = Vector2.ClampMagnitude(new Vector2(rb.velocity.x, rb.velocity.z), ps.maxMagnitude);
            rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.y);
        }
    }

    public void UpdateStats()
    {
        ps = basePlayerStats;
    }

    #region Enable and Disable Inputs

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }

    #endregion
}
