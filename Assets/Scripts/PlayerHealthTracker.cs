using MagicPigGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthTracker : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI maxHeatlhText;
    public TextMeshProUGUI maxShieldText;
    public HorizontalProgressBar healthBar;
    public HorizontalProgressBar shieldBar;

    public PlayerController player;

    private void Update()
    {
        healthText.text = Math.Round(player.ps.health, 2).ToString();
        shieldText.text = Math.Round(player.ps.shield, 2).ToString();
        maxHeatlhText.text = Math.Round(player.ps.maxHealth).ToString();
        maxShieldText.text = Math.Round(player.ps.maxShield).ToString();
        healthBar.SetProgress(player.ps.health / player.ps.maxHealth);
        shieldBar.SetProgress(player.ps.shield / player.ps.maxShield);
    }
}
