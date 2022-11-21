using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamager : MonoBehaviour
{
    public float damagePerSecond = 20;

    private float lastTime = 0;

    public float GetDamage()
    {
        if (Time.time - lastTime >= 1)
        {
            lastTime = Time.time;
            return damagePerSecond;
        }
        else
        {
            return 0;
        }
    }
}
