using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealthPoints;
    public float currentHealthPoints;
    public float maxShieldPoints;
    public float currentShieldPoints;
    public float moveSpeed;
    public float jumpHeight;
    public float spellCooldown;

    [Header("Gun Stats")]
    public float damage;
    public float fireRate;
    public float currentAmmoCount;
    public float ammoCapacity;
    public float reloadSpeed;

    [Header("UI Visuals")]
    public TMP_Text healthPoints;
    public TMP_Text ammoCount;

    void Start()
    {
        
    }

    void Update()
    {
        healthPoints.text = $"{currentHealthPoints}";
        ammoCount.text = $"{currentAmmoCount}";
    }
}
