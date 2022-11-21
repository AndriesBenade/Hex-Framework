using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPauser : MonoBehaviour
{
    public PlayerController player;
    public GameObject[] pauseObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool b = false;
        foreach (GameObject g in pauseObjects)
        {
            if (g.activeSelf)
            {
                player.force_pause = true;
                b = true;
                return;
            }
        }
        if (!b)
        {
            player.force_pause = false;
        }    
    }
}
