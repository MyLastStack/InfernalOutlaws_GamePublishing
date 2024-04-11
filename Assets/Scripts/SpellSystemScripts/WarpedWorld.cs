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
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (!affectedAgents.Contains(agent))
            {
                affectedAgents.Add(agent);
                agent.speed *= speedMultiplier;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null && affectedAgents.Contains(agent))
        {
            affectedAgents.Remove(agent);
            agent.speed /= speedMultiplier;
        }
    }

    private void OnDestroy()
    {
        // Restore original speeds of all affected agents before destruction
        foreach (NavMeshAgent agent in affectedAgents)
        {
            agent.speed /= speedMultiplier;
        }
    }
}
