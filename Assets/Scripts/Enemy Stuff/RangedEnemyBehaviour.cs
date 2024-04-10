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

    Timer attackTimer;
    public float attackCooldown;

    Timer agentUpdateTimer;
    private float refreshRate = 0.2f;

    public float damage = 5;
    public int projectiles = 8;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        attackTimer = new Timer(attackCooldown);
        attackTimer.timerComplete.AddListener(SpawnProjectiles);

        agentUpdateTimer = new Timer(refreshRate);
        agentUpdateTimer.timerComplete.AddListener(UpdateAgent);
    }

    void Update()
    {
        aimPoint.LookAt(player.transform);

        agentUpdateTimer.Tick(Time.deltaTime);

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
            attackTimer.Tick(Time.deltaTime);
        }
    }

    private void SpawnProjectiles()
    {
        attackTimer.Reset();
        float maxDeviationAngle = 10f;

        for (int i = 0; i < projectiles; i++)
        {
            float deviationAngle = Random.Range(-maxDeviationAngle, maxDeviationAngle);

            Quaternion randomRotation = Quaternion.Euler(deviationAngle, deviationAngle, 0) * aimPoint.transform.rotation;

            GameObject proj = Instantiate(enemyProjectilePrefab, aimPoint.transform.position, randomRotation);

            proj.GetComponent<EnemyAttack>().damage = damage;
        }
    }

    void UpdateAgent()
    {
        agentUpdateTimer.Reset();
        NavMeshHit navHit;
        NavMesh.SamplePosition(player.transform.position, out navHit, 50f, NavMesh.GetAreaFromName("Humanoid"));

        //Follow player
        agent.SetDestination(navHit.position);
    }
}
