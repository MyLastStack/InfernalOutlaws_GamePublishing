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

    public float damage;
    public EnemyAttack attack;

    private void Start()
    {
        attack.damage = damage;
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
        NavMeshHit navHit;
        NavMesh.SamplePosition(player.transform.position, out navHit, 50f, NavMesh.GetAreaFromName("Humanoid"));

        //Follow player
        agent.SetDestination(navHit.position);

        //If player is in range, try to attack
        if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
        {
            attackTimer.Tick(Time.deltaTime);
            //Rotate to look at the player
            Quaternion rotation = Quaternion.LookRotation(player.gameObject.transform.position - transform.position);
            transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
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
