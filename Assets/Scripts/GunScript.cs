using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager; //So so I don't have to repeat eventmanager over and over again

public class GunScript : MonoBehaviour
{
    //Firerate variables
    public float fireRate;
    float timeStamp = 0;
    public float spread;

    //Gun variables
    public LayerMask mask;
    public bool active;
    public int maxAmmo;
    int ammo;
    public bool usesAmmo;
    public float damage;
    public float range;

    //Components
    public GameObject bulletImpact;
    //public GameObject muzzleFlash;
    //public GameObject muzzleFlashPosition;
    //AudioSource src;
    public Camera cam;
    [SerializeField] InputAction fireAction;

    private void Awake()
    {
        //src = GetComponent<AudioSource>();
        ammo = maxAmmo;
    }

    private void Update()
    {
        //Okay, to prevent a bunch of layered if statements I put this all in one If statement, in total what it's checking is
        //Is the fire button pressed?
        //Is its firerate cooldown done?
        //Is it active?
        //Does it use ammo? If so, does it have any left?
        //Is the game unpaused?
        if (fireAction.IsPressed() && timeStamp + fireRate < Time.time && active && (usesAmmo && ammo > 0 || !usesAmmo) && Time.timeScale > 0)
        {
            GunFired.Invoke(this);

            timeStamp = Time.time;
            //src.Play();

            //Create ray to point towards
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            ray.direction += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));

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
                    GenericHitEntity.Invoke(hitEntity, damage);
                    if(healthScript.type == EntityType.Enemy)
                    {
                        GenericHitEnemy.Invoke(hitEntity, damage);
                        BulletHitEnemy.Invoke(this, hitEntity, damage);
                    }

                    healthScript.health -= damage;
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
            if (usesAmmo)
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
