using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public PlayerController player;
    public Animator anim;

    private void Update()
    {
        if (player.force_pause)
        {
            anim.SetBool("Move", false);
        }
        else
        {
            bool move = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow);
            bool crouch = player.getCrouch();
            anim.SetBool("Crouch", crouch);
            anim.SetBool("Move", move);
            if (!crouch)
            {
                if (move)
                {
                    bool run = player.getSprint();
                    anim.SetBool("Run", run);
                }
            }
        }
    }

}
