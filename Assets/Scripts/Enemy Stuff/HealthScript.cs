using MagicPigGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EventManager;

public class HealthScript : MonoBehaviour
{
    public float health;
    public HorizontalProgressBar bar;
    public EntityType type;
    public float maxHealth;

    private void Start()
    {
        maxHealth = health;
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (health == maxHealth)
        {
            bar.gameObject.SetActive(false);
        }
        else
        {
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 25)
            {
                bar.gameObject.SetActive(true);
            }
            else
            {
                bar.gameObject.SetActive(false);
            }
        }
        if (health <= 0)
        {
            EntityDeath.Invoke(gameObject);
            if (type == EntityType.Enemy)
            {
                EnemyDeath.Invoke(gameObject);
                Destroy(gameObject);
            }
            else if (type == EntityType.Player)
            {
                PlayerDeath.Invoke(gameObject);
            }
        }
        else
        {
            bar.SetProgress(health / maxHealth);
        }
    }
}

public enum EntityType
{
    Player,
    Enemy,
    Destructible //Might not use this one but we'll see
}
