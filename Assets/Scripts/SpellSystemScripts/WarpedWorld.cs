using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarpedWorld : MonoBehaviour
{
    public float targetScale;
    public float duration;
    public float stayDuration;
    public float speedMultiplier; // Adjust for balance!

    private Vector3 initialScale;
    private float startTime;
    private bool effectApplied = false; // Flag to track if the effect has been applied
    private List<NavMeshAgent> affectedAgents = new List<NavMeshAgent>();

    void Start()
    {
        initialScale = transform.localScale;
        startTime = Time.time;
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

    private void OnTriggerEnter(Collider other)
    {
        if (effectApplied)
            return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed *= speedMultiplier;
            affectedAgents.Add(agent);
            effectApplied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && affectedAgents.Contains(agent))
        {
            agent.speed /= speedMultiplier;
            affectedAgents.Remove(agent);

            if (affectedAgents.Count == 0)
            {
                effectApplied = false;
            }
        }
    }

    private void OnDestroy()
    {
        RestoreAgentSpeeds();
    }

    private void RestoreAgentSpeeds()
    {
        foreach (NavMeshAgent agent in affectedAgents)
        {
            agent.speed /= speedMultiplier;
        }

        affectedAgents.Clear();
    }
}
