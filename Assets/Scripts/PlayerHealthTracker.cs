using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthTracker : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;

    public PlayerController player;

    private void Update()
    {
        healthText.text = Math.Round(player.ps.health, 2).ToString();
        shieldText.text = Math.Round(player.ps.shield, 2).ToString();
    }
}
