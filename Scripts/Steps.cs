using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{

    public PlayerController player;
    public AudioSource footsteps;

    private void Update()
    {
        if (player.force_pause)
            if (footsteps.isPlaying)
                footsteps.Stop();
        if (!player.force_pause)
        {
            bool moving = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W);
            if (moving)
            {
                if (!footsteps.isPlaying)
                    footsteps.Play();
            }
            else
            {
                if (footsteps.isPlaying)
                    footsteps.Stop();
            }
        }
    }

}
