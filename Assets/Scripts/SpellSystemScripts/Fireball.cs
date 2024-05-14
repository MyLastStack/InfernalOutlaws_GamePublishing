using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    public float damage = 15f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 50f * uStats.ProjectileSpeed.Value;
        gameObject.transform.localScale *= uStats.SpellSize.Value;
    }

    void Update()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            return;

        HealthScript enemyGO = other.gameObject.GetComponent<HealthScript>();

        if (enemyGO != null)
        {
            enemyGO.health -= damage * uStats.SpellDamage.Value;
        }

        Destroy(gameObject);
    }
}
