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
        healthText.text = player.ps.health.ToString();
        shieldText.text = player.ps.shield.ToString();
    }
}
