using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager; //So so I don't have to repeat eventmanager over and over again
using Random = UnityEngine.Random;

public class GunScript : MonoBehaviour
{
    public GunStats stats;
    public LayerMask mask;
    public bool active;
    int ammo;

    //Components
    Timer timer;
    public GameObject bulletImpact;
    //public GameObject muzzleFlash;
    //public GameObject muzzleFlashPosition;
    //AudioSource src;
    public Camera cam;
    [SerializeField] InputAction fireAction;

    private void Awake()
    {
        stats.SetStats();
        //src = GetComponent<AudioSource>();
        ammo = stats.maxAmmo.iValue;
        timer = new Timer(1f / stats.fireRate.Value);
    }

    private void Update()
    {
        #region Place stat tracking debugs here

        Debug.Log(stats.fireRate.Value);

        #endregion


        timer.Tick(Time.deltaTime);
        //Okay, to prevent a bunch of layered if statements I put this all in one If statement, in total what it's checking is
        //Is the fire button pressed?
        //Is its firerate cooldown done?
        //Is it active?
        //Does it use ammo? If so, does it have any left?
        //Is the game unpaused?
        if (fireAction.IsPressed() && timer.IsDone() && active && (stats.usesAmmo && ammo > 0 || !stats.usesAmmo) && Time.timeScale > 0)
        {
            timer.Reset();
            timer.SetMaxTime(1f / stats.fireRate.Value);
            GunFired.Invoke(this);

            //src.Play();

            //Create ray to point towards
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            ray.direction += new Vector3
                (Random.Range(-stats.spread.Value, stats.spread.Value),
                Random.Range(-stats.spread.Value, stats.spread.Value),
                Random.Range(-stats.spread.Value, stats.spread.Value));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) //If it hits a target
            {
                GameObject hitEntity = hit.collider.gameObject;
                transform.LookAt(hit.point);
                if (hitEntity.tag == "Wall") //If it hit a wall/floor
                {
                    var instance = Instantiate(bulletImpact);
                    instance.transform.position = hit.point - ray.direction * 0.01f;
                    instance.transform.forward = hit.normal;
                }
                //Deal damage
                var healthScript = hit.collider.gameObject.GetComponent<HealthScript>();
                if (healthScript != null)
                {
                    //Invoke events
                    GenericHitEntity.Invoke(hitEntity, stats.damage.Value);
                    if (healthScript.type == EntityType.Enemy)
                    {
                        GenericHitEnemy.Invoke(hitEntity, stats.damage.Value);
                        BulletHitEnemy.Invoke(this, hitEntity, stats.damage.Value);
                    }

                    healthScript.health -= stats.damage.Value;
                }
            }
            else //If it doesn't hit anything
            {
                transform.forward = ray.direction;
            }

            //Create muzzle flash
            //var muzzleFlashInstance = Instantiate(muzzleFlash);
            //muzzleFlashInstance.transform.position = muzzleFlashPosition.transform.position;
            //muzzleFlashInstance.transform.rotation = transform.rotation;

            //Remove ammo
            if (stats.usesAmmo)
            {
                ammo -= 1;
            }
        }
    }

    private void OnEnable()
    {
        fireAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }
}

[Serializable]
public class GunStats
{
    public GunScriptable baseStats;

    //Firerate variables
    [HideInInspector] public Stat fireRate;
    [HideInInspector] public Stat spread;

    //Gun variables
    [HideInInspector] public Stat maxAmmo;
    [HideInInspector] public bool usesAmmo;
    [HideInInspector] public Stat damage;
    [HideInInspector] public Stat range;

    public void SetStats()
    {
        fireRate = new Stat(baseStats.fireRate);
        spread = new Stat(baseStats.spread);
        maxAmmo = new Stat(baseStats.maxAmmo);
        usesAmmo = baseStats.usesAmmo;
        damage = new Stat(baseStats.damage);
        range = new Stat(baseStats.range);
    }
}
