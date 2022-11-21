using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRender : MonoBehaviour
{
    public GameObject[] objects;
    private string TRIGGER_TAG = "Player";
    private bool started = false;
    public bool on = false;

    private void Start()
    {
        if (!on)
            disable();
        else
            enable();
        started = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!started)
        {
            if (other.CompareTag(TRIGGER_TAG))
            {
                if (!on)
                    enable();
                started = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TRIGGER_TAG))
        {
            if (!on)
                enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TRIGGER_TAG))
        {
            if (!on)
                disable();
        }
    }

    public void enable()
    {
        if (objects.Length > 0)
            foreach (GameObject g in objects)
                g.SetActive(true);
    }

    public void disable()
    {
        if (objects.Length > 0)
            foreach (GameObject g in objects)
                g.SetActive(false);
    }

}
