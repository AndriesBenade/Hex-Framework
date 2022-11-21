using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmfMeter : MonoBehaviour
{
    public Transform target;
    [Space(5)]
    public MeshRenderer led;
    public Material led_off, led_green, led_yellow, led_red;
    public AudioFix sound;
    [Space(5)]
    public float green_dist = 20;
    public float yellow_dist = 15;
    public float red_dist = 10;
    private float green_vol = 1f;
    private float yellow_vol = 2f;
    private float red_vol = 3f;

    private void Update()
    {
        float dist = (target.position - transform.position).magnitude;
        if (dist <= green_dist)
        {
            if (target.position.y >= 9.1f && transform.position.y < 9.1f)
            {
                setMode("off");
                return;
            }
            if (target.position.y <= 4.0f && transform.position.y > 9.1f)
            {
                setMode("off");
                return;
            }
            if (dist <= yellow_dist)
            {
                if (dist <= red_dist)
                {
                    setMode("red");
                }
                else
                {
                    setMode("yellow");
                }
            }
            else
            {
                setMode("green");
            }
        }
        else
        {
            setMode("off");
        }
    }

    void setMode(string mode = "off")
    {
        if (mode.ToLower() == "off")
        {
            if (sound.gameObject.GetComponent<AudioSource>().isPlaying)
                sound.gameObject.GetComponent<AudioSource>().Stop();
        }
        else
        {
            if (!sound.gameObject.GetComponent<AudioSource>().isPlaying)
                sound.gameObject.GetComponent<AudioSource>().Play();
        }
        switch (mode.ToLower())
        {
            case "off":
                led.material = led_off;
                sound.pitchMultiplier = 0;
                break;
            case "green":
                led.material = led_green;
                sound.pitchMultiplier = green_vol;
                break;
            case "yellow":
                led.material = led_yellow;
                sound.pitchMultiplier = yellow_vol;
                break;
            case "red":
                led.material = led_red;
                sound.pitchMultiplier = red_vol;
                break;
        }
    }

}
