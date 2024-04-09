using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarpedWorld : MonoBehaviour
{
    public float targetScale;
    public float duration;
    public float stayDuration;
    public float speedMultiplier = 0.5f; // Adjust as needed

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
            // Restore original speeds of all affected agents before destruction
            foreach (NavMeshAgent agent in affectedAgents)
            {
                agent.speed /= speedMultiplier;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an enemy with a NavMeshAgent component
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // Slow down the agent's speed
            agent.speed *= speedMultiplier;

            // Add the agent to the list of affected agents
            affectedAgents.Add(agent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to an enemy with a NavMeshAgent component
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // Restore the agent's original speed
            agent.speed /= speedMultiplier;

            // Remove the agent from the list of affected agents
            affectedAgents.Remove(agent);
        }
    }
}
