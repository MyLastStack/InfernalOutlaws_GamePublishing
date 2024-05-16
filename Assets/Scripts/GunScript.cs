using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    AudioSource src;
    public Camera cam;
    public PlayerController player;
    [SerializeField] InputAction fireAction;
    [SerializeField] InputAction reloadAction;
    Timer reloadTimer;

    LineRenderer line;
    Timer lineTimer;


    //For event data purposes
    [HideInInspector] public float damageDealt;

    private void Awake()
    {
        stats.SetStats();
        src = GetComponent<AudioSource>();
        ammo = stats.maxAmmo.iValue;
        timer = new Timer(1f / stats.fireRate.Value + 0.001f);
        reloadTimer = new Timer(2);
        reloadTimer.Pause();
        line = GetComponent<LineRenderer>();
        lineTimer = new Timer(1);
        lineTimer.timerComplete.AddListener(ResetLine);
    }

    private void Update()
    {
        #region Place stat tracking debugs here



        #endregion

        if (line.enabled)
        {
            lineTimer.Tick(Time.deltaTime);
        }


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
            timer.SetMaxTime(1f / stats.fireRate.Value + 0.001f);
            GunFired.Invoke(this);


            src.pitch = Random.Range(0.95f, 1.05f);
            src.Play();

            //Apply recoil
            StartCoroutine(ApplyRecoil());

            //Create ray to point towards
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            ray.direction += new Vector3
                (Random.Range(-stats.spread.Value, stats.spread.Value),
                Random.Range(-stats.spread.Value, stats.spread.Value),
                Random.Range(-stats.spread.Value, stats.spread.Value));


            lineTimer.Reset();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) //If it hits a target
            {
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);

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
                    damageDealt = stats.damage.Value;
                    //Invoke events
                    GenericHitEntity.Invoke(hitEntity, damageDealt);
                    if (healthScript.type == EntityType.Enemy)
                    {
                        GenericHitEnemy.Invoke(hitEntity, damageDealt);
                        BulletHitEnemy.Invoke(this, hitEntity, damageDealt);
                    }

                    StartCoroutine(ApplyRedFlash(hit.collider.gameObject));
                    healthScript.health -= damageDealt;
                }
            }
            else //If it doesn't hit anything
            {
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position + ray.direction * 50);

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

        reloadTimer.Tick(Time.deltaTime);

        if (reloadAction.WasPressedThisFrame() && ammo <= 0)
        {
            reloadTimer.Unpause();
        }

        if (reloadTimer.IsDone())
        {
            ammo = stats.maxAmmo.iValue;
            reloadTimer.Reset();
            reloadTimer.Pause();
        }
    }

    IEnumerator ApplyRedFlash(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
        }
        yield return new WaitForSeconds(0.1f);

        if (obj != null)
        {
            obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }

    public IEnumerator ApplyRecoil()
    {
        player.targetXRotation += -stats.recoil.Value * 0.33f;
        yield return new WaitForSeconds(0.025f);
        player.targetXRotation += -stats.recoil.Value * 0.33f;
        yield return new WaitForSeconds(0.025f);
        player.targetXRotation += -stats.recoil.Value * 0.33f;

        float easeDown = 0.25f;
        while(easeDown > 0)
        {
            easeDown -= 0.025f;
            player.targetXRotation += 0.025f * stats.recoil.Value / 2;
            yield return new WaitForSeconds(0.025f);
        }
    }

    void ResetLine()
    {
        line.enabled = false;

        lineTimer.Reset();
    }

    private void OnEnable()
    {
        fireAction.Enable();
        reloadAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
        reloadAction.Disable();
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
    [HideInInspector] public Stat recoil;

    public void SetStats()
    {
        fireRate = new Stat(baseStats.fireRate);
        spread = new Stat(baseStats.spread);
        maxAmmo = new Stat(baseStats.maxAmmo);
        usesAmmo = baseStats.usesAmmo;
        damage = new Stat(baseStats.damage);
        range = new Stat(baseStats.range);
        recoil = new Stat(baseStats.recoil);
    }
}
