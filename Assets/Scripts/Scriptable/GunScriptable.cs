using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class GunScriptable : ScriptableObject
{
    //Firerate variables
    public float fireRate;
    public float spread;

    //Gun variables
    public int maxAmmo;
    public bool usesAmmo;
    public float damage;
    public float range;
}
