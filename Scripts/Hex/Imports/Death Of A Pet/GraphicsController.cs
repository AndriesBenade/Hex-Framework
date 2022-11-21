using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsController : MonoBehaviour
{
    //public float speed = 1;
    public Animator anim;
    public PlayerController player;
    public AudioSource footsteps;

    private bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            paused = paused ? false : true;
        if (Time.timeScale != 0)
            paused = false;
        if (player.force_pause)
            anim.SetBool("Moving", false);
        if (player.force_pause)
            if (footsteps.isPlaying)
                footsteps.Stop();
        if (!paused && !player.force_pause)
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
            anim.SetBool("Moving", moving);
            anim.SetBool("Running", Input.GetKey(KeyCode.LeftShift));
            float x = 0;
            if (Input.GetKey(KeyCode.A))
            {
                x = -90;
                if (Input.GetKey(KeyCode.W))
                    x = -45;
                if (Input.GetKey(KeyCode.S))
                    x = -135;
                transform.localEulerAngles = new Vector3(0, x, 0);
                return;
            }
            if (Input.GetKey(KeyCode.W))
            {
                x = 0;
                if (Input.GetKey(KeyCode.D))
                    x = 45;
                transform.localEulerAngles = new Vector3(0, x, 0);
                return;
            }
            if (Input.GetKey(KeyCode.S))
            {
                x = 180;
                if (Input.GetKey(KeyCode.D))
                    x = 135;
                transform.localEulerAngles = new Vector3(0, x, 0);
                return;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x = 90;
                transform.localEulerAngles = new Vector3(0, x, 0);
                return;
            }
        }
    }
}
