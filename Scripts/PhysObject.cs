using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysObject : MonoBehaviour
{
    public bool enableImpactSounds = false;
    public float impactForce = 0.3f;
    public AudioSource audioPlayer;
    public AudioClip[] impactSounds;
    public float impactInterval = 0.5f;
    [Space(5)]
    public bool autoDisable = false;
    public float disableInterval = 3;
    [Space(5)]
    public bool allowPhysGrab = false;

    private Rigidbody rb;
    private float lastTimeSound = 0;
    private float lastTimeImpact = 0;
    private bool affected = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float force = (collision.impulse.x + collision.impulse.y + collision.impulse.z) / ((collision.impulse.x > 0 ? 1 : 0) + (collision.impulse.y > 0 ? 1 : 0) + (collision.impulse.z > 0 ? 1 : 0));
        if (force >= impactForce)
        {
            lastTimeImpact = Time.time;
            affected = true;

            if (enableImpactSounds)
            {
                if (Time.time - lastTimeSound >= impactInterval)
                {
                    lastTimeSound = Time.time;
                    if (impactSounds.Length > 1)
                    {
                        int rnd = Random.Range(0, impactSounds.Length);
                        while (audioPlayer.clip == impactSounds[rnd])
                        {
                            rnd = Random.Range(0, impactSounds.Length);
                        }
                        audioPlayer.clip = impactSounds[rnd];
                    }
                    else
                    {
                        audioPlayer.clip = impactSounds[0];
                    }
                    audioPlayer.Play();
                }
            }
        }
    }

    private void Update()
    {
        if (autoDisable && affected)
        {
            if (Time.time - lastTimeImpact >= disableInterval)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void freeze()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = c.enabled ? false : true;
        }
    }

    public void unfreeze()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = c.enabled ? false : true;
        }
    }

    public void shoot(Vector3 force)
    {
        rb.AddForce(force);
    }    

}
