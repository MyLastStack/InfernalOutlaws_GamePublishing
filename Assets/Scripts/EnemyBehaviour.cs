using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    [SerializeField]
    GameObject attackBox;
    public float attackRange;

    public float attackCooldown;
    Timer attackTimer;

    public float attackDuration;
    Timer attackDurationTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        //Setup timers
        attackTimer = new Timer(attackCooldown);
        attackTimer.timerComplete.AddListener(StartAttack);
        attackDurationTimer = new Timer(attackDuration);
        attackDurationTimer.timerComplete.AddListener(EndAttack);
    }

    private void Update()
    {
        //Follow player
        agent.SetDestination(player.transform.position);

        //If player is in range, try to attack
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            attackTimer.Tick(Time.deltaTime);
        }
        else //Otherwise reset attack timer back to its max
        {
            attackTimer.Reset();
        }

        //If attacking, count down attack time left
        if (attackBox.activeInHierarchy)
        {
            attackDurationTimer.Tick(Time.deltaTime);
        }
    }

    private void StartAttack()
    {
        attackBox.SetActive(true);
        attackTimer.Reset();
    }

    private void EndAttack()
    {
        attackBox.SetActive(false);
        attackDurationTimer.Reset();
    }
}
