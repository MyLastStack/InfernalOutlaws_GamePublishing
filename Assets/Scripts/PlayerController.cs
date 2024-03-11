using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager;

public class PlayerController : MonoBehaviour
{
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

    //Dash variables
    public float dashTime; //How long does the dash last
    float dashTimeLeft; //A tracker for dash time.
    public float dashCooldown; //How long after a dash before the player can dash again
    float dashCooldownTimeLeft; //A tracker for dash cooldown
    public float dashForce; //How much force is applied during the dash.
    public float dashMaxMagnitude; //The max magnitude of the player during the dash.
    public float dashMoveSpeed; //Control the player has when dashing
    public float dashDeccelRate = 1.05f; //DeccelRate during dash
    public float dashFOV; //Camera FOV during dash

    //Movement Variables
    public float moveSpeed; //The speed at which the player accelerates
    public float walkMoveSpeed; //Control the player has when walking
    public float rotateSpeed; //Camera sensitivity
    public float jumpPower; //How much force is applied during a jump
    public float walkMaxMagnitude; //Max magnitude while not dashing
    public float maxMagnitude; //The max speed the player can move at
    public float deccelRate = 1.1f; //Essentially simulated friction, the rate the player deccelerates when not moving in a given direction
    public float walkDeccelRate = 1.1f; //Deccel rate during walk
    public float walkFOV = 60;

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
        rb = GetComponent<Rigidbody>();
        targetXRotation = camPivot.transform.localRotation.x;
        targetYRotation = camPivot.transform.localRotation.y;
        Physics.gravity *= 4;
    }

    void Update()
    {
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
            targetXRotation -= Input.GetAxis("Mouse Y") * rotateSpeed;
            targetYRotation += Input.GetAxis("Mouse X") * rotateSpeed;
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
        if (dashTimeLeft > 0)
        {
            dashTimeLeft -= Time.deltaTime;
            if(cam.fieldOfView < dashFOV)
            {
                cam.fieldOfView += FOVChangeRate * Time.deltaTime;
            }
        }
        else
        {
            maxMagnitude = walkMaxMagnitude;
            moveSpeed = walkMoveSpeed;
            deccelRate = walkDeccelRate;
            if (cam.fieldOfView > walkFOV)
            {
                cam.fieldOfView -= FOVChangeRate * Time.deltaTime;
            }
        }

        if (dashCooldownTimeLeft > 0)
        {
            dashCooldownTimeLeft -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (dashAction.IsPressed() && hasControl && dashCooldownTimeLeft <= 0)
        {
            PlayerDash.Invoke(gameObject);
            maxMagnitude = dashMaxMagnitude;
            rb.AddForce(transform.forward * dashForce);
            dashTimeLeft = dashTime;
            dashCooldownTimeLeft = dashCooldown;
            moveSpeed = dashMoveSpeed;
            deccelRate = dashDeccelRate;
        }

        if (jumpAction.IsPressed() && onGround && hasControl)
        {
            Jump.Invoke(gameObject);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(jumpPower * Vector3.up);
        }
    }

    public void Movement()
    {
        //First, apply the force
        rb.AddRelativeForce(new Vector3(moveValue.x * moveSpeed, 0, moveValue.y * moveSpeed));

        if (moveValue.x == 0)
        {
            //This dampens x velocity when left and right aren't held. It first converts to local velocity, reduces it, then converts it back to global velocity.
            var localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity = new Vector3(localVelocity.x / deccelRate, localVelocity.y, localVelocity.z);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
        if (moveValue.y == 0)
        {
            //This does the same but for z velocity
            var localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity = new Vector3(localVelocity.x, localVelocity.y, localVelocity.z / deccelRate);
            rb.velocity = transform.TransformDirection(localVelocity);
        }
        if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude > maxMagnitude)
        {
            //Finally, it clamps the magnitude of the velocity so that the player doesn't accelerate to infinity
            var clampedVelocity = Vector2.ClampMagnitude(new Vector2(rb.velocity.x, rb.velocity.z), maxMagnitude);
            rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.y);
        }
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
