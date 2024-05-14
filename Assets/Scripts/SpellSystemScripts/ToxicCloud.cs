using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCloud : Spell
{
    public float targetScale;
    public float growTime;
    public float duration;
    public float damageInterval;
    public float damage;

    private Vector3 initialScale;
    private float startTime;

    private List<HealthScript> affectedEnemies = new List<HealthScript>();

    void Start()
    {
        initialScale = transform.localScale;
        startTime = Time.time;
        StartCoroutine(ApplyDamageRoutine());
    }

    void Update()
    {
        float progress = (Time.time - startTime) / growTime;
        progress = Mathf.Clamp01(progress);
        Vector3 newScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale) * uStats.SpellSize.Value, progress);
        transform.localScale = newScale;

        if (progress >= 1f)
        {
            Destroy(gameObject, duration * uStats.Duration.Value);
        }
    }

    private IEnumerator ApplyDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval / uStats.TickRate.Value);

            affectedEnemies.RemoveAll(x => x == null);
            
            foreach (HealthScript enemy in affectedEnemies)
            {
                if (enemy != null)
                {
                    enemy.health -= damage * uStats.SpellDamage.Value;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthScript enemyHealth = other.gameObject.GetComponent<HealthScript>();
        if (enemyHealth != null && !affectedEnemies.Contains(enemyHealth))
        {
            affectedEnemies.Add(enemyHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HealthScript enemyHealth = other.gameObject.GetComponent<HealthScript>();
        if (enemyHealth != null && affectedEnemies.Contains(enemyHealth))
        {
            affectedEnemies.Remove(enemyHealth);
        }
    }
}
