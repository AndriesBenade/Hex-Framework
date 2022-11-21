using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFix : MonoBehaviour
{

    public float volume = 1f;
    public float maxDistance = 5;
    public bool maintainPitch = false;
    public float pitchMultiplier = 1;

    private AudioSource aud;
    private Transform player;
    private float pitch = 1;

    private void Start()
    {
        if (aud == null)
        {
            aud = GetComponent<AudioSource>();
            pitch = aud.pitch;
        }
        if (player == null)
            player = FindObjectOfType<AudioListener>().gameObject.transform;
    }

    private void Update()
    {
        if (!maintainPitch)
            aud.pitch = pitch * pitchMultiplier * Time.timeScale;
        aud.volume = getVolume();
    }

    private float getVolume()
    {
        if (maxDistance != -1)
        {
            float x = 0;
            float dist = Vector3.Magnitude(transform.position - player.position) / 10;
            if (dist <= maxDistance)
                x = volume * (1 - dist / maxDistance);
            //Debug.Log("Vol: " + x + "; Dist: " + dist);
            return x;
        }
        else
        {
            return volume;
        }
    }

}
