using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileAttack : MonoBehaviour
{
    private float damageDeal = 5f;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = transform.forward * 50f;
    }

    void Update()
    {
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

        if (pc != null)
        {
            pc.ps.health -= damageDeal;
        }

        Destroy(gameObject);
    }
}
