using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTracker : MonoBehaviour
{
    public float delay = 0;
    public FlowEvent[] onFinish;
    public GameObject[] enableOnFinish;
    private bool resetOnFinish = false;

    private AudioSource aud;
    private float startTime = 0;
    private bool started = false;

    void Start()
    {
        if (aud == null)
            aud = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!started)
        {
            if (aud.isPlaying)
            {
                startTime = Time.time;
                started = true;
            }
        }
        else
        {
            if (Time.time - startTime >= aud.clip.length)
            {
                StartCoroutine(IRun());
            }
        }
    }

    private IEnumerator IRun()
    {
        yield return new WaitForSeconds(delay);
        foreach (FlowEvent e in onFinish)
        {
            e.runEvent();
        }
        foreach (GameObject g in enableOnFinish)
        {
            g.SetActive(true);
        }
        if (resetOnFinish)
        {
            started = false;
        }
    }
}
