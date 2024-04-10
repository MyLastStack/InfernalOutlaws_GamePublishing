using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float damageDeal = 15f;

    private PlayerController playerScript;

    Rigidbody rb;

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 50f;
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
            enemyGO.health -= damageDeal * (playerScript.gun.stats.damage.Value / playerScript.gun.stats.damage.BaseValue);
        }

        Destroy(gameObject);
    }
}
