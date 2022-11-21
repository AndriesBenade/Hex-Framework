using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class RandomAnimTrigger : MonoBehaviour
{

    public string[] triggers;
    [Space(5)]
    public bool useAudio = false;
    public AudioSource aud;
    [Space(5)]
    public bool enableCooldown = false;
    public float cooldown = 0.5f;

    private Animator anim;
    private float lastTime;
    private bool halt = false;
    private PlayerController player;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            halt = halt ? false : true;
        if (!halt && !player.force_pause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (enableCooldown)
                {
                    if (Time.time - lastTime < cooldown)
                        return;
                }
                int rand = UnityEngine.Random.Range(0, triggers.Length);
                anim.SetTrigger(triggers[rand]);
                lastTime = Time.time;
                if (useAudio)
                    aud.Play();
            }
        }
    }

}
