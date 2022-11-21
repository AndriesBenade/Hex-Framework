using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobAnim : MonoBehaviour
{

    public Animator anim;
    public string moving = "";
    public string running = "";
    public string crouch = "";

    private PlayerController player;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        anim.SetBool(moving, Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
        anim.SetBool(running, Input.GetKey(KeyCode.LeftShift) && !player.isTired());
        anim.SetBool(crouch, player.isCrouching());
    }
}
