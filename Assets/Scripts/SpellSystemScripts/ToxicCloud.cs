using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    public float targetScale;
    public float duration;
    public float stayDuration;

    private Vector3 initialScale;
    private float startTime;

    private float damageDeal = 2.5f;

    private PlayerController playerScript;
    private Timer timer;

    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        initialScale = transform.localScale;
        startTime = Time.time;

        timer = new Timer(0.5f);
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

        timer.Tick(Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        HealthScript enemyGO = other.gameObject.GetComponent<HealthScript>();

        if (enemyGO != null)
        {
            if (timer.IsDone())
            {
                timer.Reset();

                enemyGO.health -= damageDeal /** (playerScript.gun.stats.damage.Value / playerScript.gun.stats.damage.BaseValue)*/;
            }
        }
    }
}
