using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public UniversalSpellStats uStats;
    public PlayerController playerScript;

    private void Awake()
    {
        uStats = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UniversalSpellStats>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}