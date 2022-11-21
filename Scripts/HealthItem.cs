using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{

    public string invID;
    public InventoryManager inv;
    public HealthManager health;
    public AudioSource aud;
    public float restorePercentRaw = 1;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            health.health += health.maxHealth * restorePercentRaw;
            int index = inv.GetItem(invID);
            if (index != -1)
            {
                inv.items[index].deduct();
                if (index == inv.index)
                {
                    inv.NextItem();
                }
            }
            aud.Play();
        }
    }

}
