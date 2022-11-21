using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayText : MonoBehaviour
{

    public GameObject display;
    public Text text;

    private PlayerController player;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }

    public void set(string s)
    {
        text.text = s;
        display.SetActive(true);
        player.force_pause = true;
    }

    public void clear()
    {
        text.text = "";
        display.SetActive(false);
        player.force_pause = false;
    }

}
