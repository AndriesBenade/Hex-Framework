using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedSteps : MonoBehaviour
{
    public AudioSource left;
    public AudioSource right;
    [Space(5)]
    public AudioClip[] steps;
    public float interval = 0.65f;
    public float runMultiplier = 0.6f;
    public float crouchMultiplier = 1.5f;

    private float lastTime = 0;
    private bool crouch = false;
    private int lastFoot = 0;
    private PlayerController player;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            crouch = crouch ? false : true;
        float modInterval = interval * (crouch ? crouchMultiplier : 1) * ((Input.GetKey(KeyCode.LeftShift) && !player.isTired()) ? runMultiplier : 1);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Time.time - lastTime >= modInterval)
            {
                if (lastFoot == 0)
                {
                    left.clip = getStep();
                    left.Play();
                    lastFoot = 1;
                    lastTime = Time.time;
                }
                else
                {
                    right.clip = getStep();
                    right.Play();
                    lastFoot = 0;
                    lastTime = Time.time;
                }
            }
        }
    }

    private AudioClip getStep()
    {
        return (steps[Random.Range(0, steps.Length)]);
    }
}
