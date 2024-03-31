using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static EventManager;

public class PlayerController : MonoBehaviour
{
    public PlayerStats ps; //I gave it a simple name because it appears EVERYWHERE in the code


    public List<Card> cards = new List<Card>(); //Replace this later

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

    public Timer dashTimer;
    public Timer dashCooldownTimer;

    public Timer shieldCooldownTimer;

    public GunScript gun; //For the purpose of referencing in cards
    public bool isDashing = false;


    //public float walkIncrements = 0.8f;
    //float walkTime = 0.5f;

    void Start()
    {
        MouseLocker.Lock();
        rb = GetComponent<Rigidbody>();
        targetXRotation = camPivot.transform.localRotation.x;
        targetYRotation = camPivot.transform.localRotation.y;
        Physics.gravity *= 4;

        dashTimer = new Timer(ps.dashTime);
        dashTimer.SetTime(0);
        dashCooldownTimer = new Timer(ps.dashCooldown);
        dashCooldownTimer.SetTime(0);
        shieldCooldownTimer = new Timer(ps.shieldCooldown);
        shieldCooldownTimer.SetTime(0);

        ps.health = ps.maxHealth;
        ps.shield = ps.maxShield;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene("MainMenu"); } //Quit game

        if (Time.timeScale == 0)
        {
            hasControl = false;
        }
        else
        {
            hasControl = true;
        }

        var onGroundLastFrame = onGround;
        RaycastHit groundHit;
        Physics.BoxCast(playerModel.transform.position, new Vector3(1, 0.1f, 1) * 2, Vector3.down, out groundHit, Quaternion.identity, groundDist, layerMask);
        onGround = groundHit.collider != null && !groundHit.collider.isTrigger; //Prevent the player from jumping on triggers
        if (!onGroundLastFrame && onGround)
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
        if (!dashTimer.IsDone())
        {
            dashTimer.Tick(Time.deltaTime);
            if (cam.fieldOfView < ps.dashFOV)
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
                if (isDashing)
                {
                    isDashing = false;
                    PlayerDashEnd.Invoke(gameObject);
                }
            }
        }

        if (!dashCooldownTimer.IsDone())
        {
            dashCooldownTimer.Tick(Time.deltaTime);
        }

        Debug.Log(shieldCooldownTimer.IsDone());
        //Check if shield should regenerate
        if (!shieldCooldownTimer.IsDone())
        {
            shieldCooldownTimer.Tick(Time.deltaTime);
        }
        else
        {
            ps.shield = Mathf.Clamp(ps.shield + (Time.deltaTime * ps.shieldRegenSpeed), 0, ps.maxShield);
        }


        if(ps.health <= 0)
        {
            MouseLocker.Unlock();
            SceneManager.LoadScene("GameOver");
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (dashAction.IsPressed() && hasControl && dashCooldownTimer.IsDone())
        {
            isDashing = true;
            PlayerDash.Invoke(gameObject);
            ps.maxMagnitude = ps.dashMaxMagnitude;
            rb.AddForce(transform.forward * ps.dashForce);
            dashTimer.Reset();
            dashCooldownTimer.Reset();
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

    private void OnTriggerEnter(Collider other)
    {
        EnemyAttack atk = other.GetComponent<EnemyAttack>();
        if (atk != null) //If hit by an enemy attack
        {
            //Reset the shield
            shieldCooldownTimer.SetMaxTime(ps.shieldCooldown);
            shieldCooldownTimer.Reset();

            //Invoke events
            GenericHitPlayer.Invoke(gameObject, atk.damage);
            GenericHitHealth.Invoke(gameObject, atk.damage);

            //Check if player is taking shield damage or health damage
            if (ps.shield > 0)
            {
                ps.shield = Mathf.Clamp(ps.shield - atk.damage, 0, ps.maxShield);

                if (ps.shield <= 0)
                {
                    ShieldBreak.Invoke(gameObject);
                }
            }
            else
            {
                ps.health = Mathf.Clamp(ps.health - atk.damage, 0, ps.maxHealth);
            }
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

//Player Stats
[Serializable]
public class PlayerStats
{
    [Header("Modifyable Stats")]
    [Space]
    public Stat dashTimeStat;
    public Stat dashCooldownStat;
    public Stat dashMaxMagnitudeStat;
    public Stat dashMoveSpeedStat;
    public Stat walkMoveSpeedStat;
    public Stat jumpPowerStat;
    public Stat walkMaxMagnitudeStat;
    public Stat maxHealthStat;
    public Stat maxShieldStat;
    public Stat shieldCooldownStat;
    public Stat shieldRegenSpeedStat;


    //The Hide in Inspector variables are mostly here to save me some re-coding, as they are just references to the stats above
    //The non-hide in inspector variables are variables that do not have a stat attached to them, meaning they cannot have modifiers applied to them.

    //Dash variables
    [HideInInspector] public float dashTime { get { return dashTimeStat.Value; } set { dashTimeStat.SetBaseValue(value); } } //How long does the dash last
    [HideInInspector] public float dashCooldown { get { return dashCooldownStat.Value; } set { dashCooldownStat.SetBaseValue(value); } } //How long after a dash before the player can dash again
    [HideInInspector] public float dashMaxMagnitude { get { return dashMaxMagnitudeStat.Value; } set { dashMaxMagnitudeStat.SetBaseValue(value); } } //The max magnitude of the player during the dash.
    [HideInInspector] public float dashMoveSpeed { get { return dashMoveSpeedStat.Value; } set { dashMoveSpeedStat.SetBaseValue(value); } } //Control the player has when dashing
    [Header("Unmodifyable Stats")]
    [Space]
    public float dashForce; //How much force is applied during the dash.   
    public float dashDeccelRate = 1.05f; //DeccelRate during dash
    public float dashFOV; //Camera FOV during dash

    //Movement Variables
    [HideInInspector] public float walkMoveSpeed { get { return walkMoveSpeedStat.Value; } set { walkMoveSpeedStat.SetBaseValue(value); } } //Control the player has when walking
    [HideInInspector] public float jumpPower { get { return jumpPowerStat.Value; } set { jumpPowerStat.SetBaseValue(value); } } //How much force is applied during a jump
    [HideInInspector] public float walkMaxMagnitude { get { return walkMaxMagnitudeStat.Value; } set { walkMaxMagnitudeStat.SetBaseValue(value); } } //Max magnitude while not dashing
    public float maxMagnitude; //The max speed the player can move at (modified by walk max magnitude and dash max magnitude)
    public float moveSpeed; //The speed at which the player accelerates (modified by walk and dash move speed)
    public float rotateSpeed; //Camera sensitivity
    public float deccelRate = 1.1f; //Essentially simulated friction, the rate the player deccelerates when not moving in a given direction
    public float walkDeccelRate = 1.1f; //Deccel rate during walk
    public float walkFOV = 60;

    //Player resources
    [HideInInspector] public float health;
    [HideInInspector] public float maxHealth { get { return maxHealthStat.Value; } set { maxHealthStat.SetBaseValue(value); } }
    [HideInInspector] public float shield;
    [HideInInspector] public float maxShield { get { return maxShieldStat.Value; } set { maxShieldStat.SetBaseValue(value); } }
    [HideInInspector] public float shieldCooldown { get { return shieldCooldownStat.Value; } set { maxShieldStat.SetBaseValue(value); } }
    [HideInInspector] public float shieldRegenSpeed { get { return shieldRegenSpeedStat.Value; } set { shieldRegenSpeedStat.SetBaseValue(value); } }

    //public void Clear() //WARNING: Will reset all item stat modifications!
    //{
    //    dashTimeStat.ClearModifiers();
    //    dashCooldownStat.ClearModifiers();
    //    dashMaxMagnitudeStat.ClearModifiers();
    //    dashMoveSpeedStat.ClearModifiers();
    //    walkMoveSpeedStat.ClearModifiers();
    //    jumpPowerStat.ClearModifiers();
    //    walkMaxMagnitudeStat.ClearModifiers();
    //}
}
