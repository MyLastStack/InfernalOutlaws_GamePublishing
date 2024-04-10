using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    public float targetScale;
    public float duration;
    public float stayDuration;
    public float damageInterval = 0.5f;
    public float damagePerInterval = 2.5f;

    private Vector3 initialScale;
    private float startTime;

    private List<HealthScript> affectedEnemies = new List<HealthScript>();
    private Coroutine damageCoroutine;
    private PlayerController playerScript;

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        initialScale = transform.localScale;
        startTime = Time.time;
        StartCoroutine(ApplyDamageRoutine());
    }

    void Update()
    {
        float progress = (Time.time - startTime) / duration;
        progress = Mathf.Clamp01(progress);
        Vector3 newScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale), progress);
        transform.localScale = newScale;

        if (progress >= 1f)
        {
            Destroy(gameObject, stayDuration);
        }
    }

    private IEnumerator ApplyDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);

            foreach (HealthScript enemy in affectedEnemies)
            {
                if (enemy != null)
                {
                    enemy.health -= damagePerInterval * (playerScript.gun.stats.damage.Value / playerScript.gun.stats.damage.BaseValue);
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
