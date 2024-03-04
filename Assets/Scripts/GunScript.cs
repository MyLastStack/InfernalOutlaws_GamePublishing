using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    //Firerate variables
    public float fireRate;
    float timeStamp;
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
    public GameObject muzzleFlash;
    public GameObject muzzleFlashPosition;
    AudioSource src;
    public Camera cam;
    [SerializeField] InputAction fireAction;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
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
            timeStamp = Time.time;
            src.Play();

            //Create ray to point towards
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            ray.direction += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) //If it hits a target
            {
                transform.LookAt(hit.point);
                var instance = Instantiate(bulletImpact);
                instance.transform.position = hit.point;
                instance.transform.forward = hit.normal;
                //Deal damage
                var healthScript = hit.collider.gameObject.GetComponent<HealthScript>();
                if (healthScript != null)
                {
                    healthScript.health -= damage;
                }
            }
            else //If it doesn't hit anything
            {
                transform.forward = ray.direction;
            }

            //Create muzzle flash
            var muzzleFlashInstance = Instantiate(muzzleFlash);
            muzzleFlashInstance.transform.position = muzzleFlashPosition.transform.position;
            muzzleFlashInstance.transform.rotation = transform.rotation;
            
            //Remove ammo
            if(usesAmmo)
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
