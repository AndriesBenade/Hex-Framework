using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using HexRepository;

[RequireComponent(typeof(Text))]
public class HealthBoundText : MonoBehaviour
{
    public HealthManager health;
    public int healthBarSize = 30;

    private Text t;
    private void Start()
    {
        if (t == null)
            t = GetComponent<Text>();
    }

    void Update()
    {
        t.text = presentation.getProgressbar(health.health / health.maxHealth, healthBarSize);
    }
}
