using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexRepository;

public class TextModifier : MonoBehaviour
{
    public PlayerController player;
    public HealthManager playerHealth;
    [Space(5)]
    public Text health;
    public Text stamina;

    void Update()
    {
        health.text = presentation.getProgressbar(playerHealth.health/playerHealth.maxHealth);
        stamina.text = presentation.getProgressbar(player.getStamina()/player.maxStamina);
    }
}
