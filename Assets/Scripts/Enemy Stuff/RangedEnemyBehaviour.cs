using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class RangedEnemyBehaviour : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    [SerializeField] Transform aimPoint;
    [SerializeField] GameObject enemyProjectilePrefab;
    public float attackRange;

    public float attackTimer;
    float loadTimer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        loadTimer = 5f;
        attackTimer = loadTimer;
    }

    void Update()
    {
        aimPoint.LookAt(player.transform);

        agent.SetDestination(player.transform.position);

        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Ensure the agent isn't looking directly up or down
        if (direction != Vector3.zero)
        {
            // Rotate the agent to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.acceleration);
        }
        
        if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
        {
            if (attackTimer <= 0)
            {
                SpawnProjectiles();
                attackTimer = loadTimer;
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void SpawnProjectiles()
    {
        float maxDeviationAngle = 10f;

        for (int i = 0; i < 5; i++)
        {
            float deviationAngle = Random.Range(-maxDeviationAngle, maxDeviationAngle);

            Quaternion randomRotation = Quaternion.Euler(deviationAngle, deviationAngle, 0) * aimPoint.transform.rotation;

            Instantiate(enemyProjectilePrefab, aimPoint.transform.position, randomRotation);
        }
    }
}
