using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitScanGun : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;
    public PlayerStats playerStats;

    [Header("Weapon Stats")]
    public float gunDamage;
    public float gunRange = 100f;
    private float nextTimeToFire = 0f;

    private bool isReloading;

    [Header("Keybinds")]
    public InputAction fireAction;
    public InputAction reloadAction;

    void Start()
    {

    }

    void Update()
    {
        PressedReload();

        if (fireAction.IsInProgress() && Time.time >= nextTimeToFire)
        {
            if (playerStats.currentAmmoCount != 0)
            {
                nextTimeToFire = Time.time + 1f / playerStats.fireRate;

                Shoot();
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange))
        {
            Debug.Log(hit.transform.name);
        }

        playerStats.currentAmmoCount--;
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(playerStats.reloadSpeed);

        playerStats.currentAmmoCount = playerStats.ammoCapacity;
        isReloading = false;
    }

    private void PressedReload()
    {
        if (isReloading)
        {
            return;
        }

        if (reloadAction.WasPressedThisFrame() && playerStats.currentAmmoCount < playerStats.ammoCapacity)
        {
            StartCoroutine(Reload());
            return;
        }
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
