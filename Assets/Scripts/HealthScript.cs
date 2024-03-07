using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EventManager;

public class HealthScript : MonoBehaviour
{
    public float health;
    public TextMeshProUGUI healthText;
    public EntityType type;

    private void Update()
    {
        healthText.text = health.ToString();
        if (health <= 0)
        {
            EntityDeath.Invoke(gameObject);
            if (type == EntityType.Enemy)
            {
                EnemyDeath.Invoke(gameObject);
                gameObject.SetActive(false);
            }
            else if (type == EntityType.Player)
            {
                PlayerDeath.Invoke(gameObject);
            }
        }
    }
}

public enum EntityType
{
    Player,
    Enemy,
    Destructible //Might not use this one but we'll see
}
