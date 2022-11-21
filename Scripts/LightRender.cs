using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Light))]
public class LightRender : MonoBehaviour
{
    public float distance = 15;
    private Light lit;
    private Transform target;

    private void Start()
    {
        if (lit == null)
            lit = GetComponent<Light>();
        if (target == null)
            target = FindObjectOfType<AudioListener>().transform;
    }

    void Update()
    {
        if (Vector3.Magnitude(transform.position - target.position) <= distance)
        {
            lit.enabled = true;
        }
        else
        {
            lit.enabled = false;
        }
    }
}
