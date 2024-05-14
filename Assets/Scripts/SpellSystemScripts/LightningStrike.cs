using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : Spell
{
    public float targetScale = 2f;
    public float duration = 0.2f;
    public float damageRadius = 5f;
    public LayerMask enemyLayer;

    private Vector3 initialScale;
    private float startTime;
    public float damage = 10f;

    private bool canDamage = true;
    private float damageCooldown = 0.1f;

    void Start()
    {
        initialScale = transform.localScale;
        startTime = Time.time;
    }

    void Update()
    {
        float progress = (Time.time - startTime) / duration;
        progress = Mathf.Clamp01(progress);
        Vector3 newScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale) * uStats.SpellSize.Value, progress);
        transform.localScale = newScale;

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, enemyLayer);

            foreach (Collider collider in colliders)
            {
                HealthScript enemyHealth = collider.GetComponent<HealthScript>();
                if (enemyHealth != null)
                {
                    enemyHealth.health -= damage * uStats.SpellDamage.Value;
                }
            }

            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}
