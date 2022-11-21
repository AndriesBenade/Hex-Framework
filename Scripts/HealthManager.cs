using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ioFileSrc;

public class HealthManager : MonoBehaviour
{

    public float health = 100;
    public float maxHealth = 100;
    [Space(5)]
    public string damageTrigger = "";
    [Space(5)]
    public bool allowRegeneration = false;
    public float cooldown = 10;
    public float hpPerSec = 2;
    [Space(5)]
    public bool isPlayer = false;
    [Space(5)]
    public bool allowAudio = false;
    public AudioSource aud;
    public AudioClip[] damageSounds;
    [Space(5)]
    public FlowEvent[] onDeath;
    public GameObject[] loot;
    public bool useEffects = false;
    public string onDeathEffect = "";
    public bool disableAI = false;

    private float lastTime = 0;
    private float cooldownStart = 0;
    private bool dead = false;
    private string healthFile = "health.flow";
    private bool halt = false;
    private PlayerController player;
    private bool cheat = false;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        if (isPlayer)
            loadHealth();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            halt = halt ? false : true;
        //if (Input.GetKeyDown(KeyCode.Backspace))
        //    cheat = cheat ? false : true;
        if (cheat && isPlayer)
            return;
        if (!dead && !halt && !player.force_pause)
        {
            health = health < 0 ? 0 : (health > maxHealth ? maxHealth : health);
            if (health <= 0)
            {
                dead = true;
                if (isPlayer)
                    player.force_pause = true;
                if (onDeath.Length > 0)
                    foreach (FlowEvent fe in onDeath)
                        fe.runEvent();
                if (loot.Length > 0)
                    foreach (GameObject go in loot)
                    {
                        go.transform.SetParent(null);
                        go.SetActive(true);
                    }
                if (useEffects)
                    FindObjectOfType<EffectManager>().PlayEffect(onDeathEffect, transform.position);
                if (disableAI)
                    if (GetComponent<NavMeshAgent>() != null)
                        GetComponent<NavMeshAgent>().enabled = false;
                return;
            }
            if (allowRegeneration && health != maxHealth)
            {
                if (Time.time - cooldownStart >= cooldown)
                {
                    if (Time.time - lastTime >= 1)
                    {
                        lastTime = Time.time;
                        health += hpPerSec;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(damageTrigger))
        {
            Debug.Log("hit");
            if (other.gameObject.GetComponent<HealthDamager>() != null)
            {
                float dmg = other.gameObject.GetComponent<HealthDamager>().GetDamage();
                health -= dmg;
                if (allowAudio && dmg > 0)
                {
                    if (aud.isPlaying)
                        aud.Stop();
                    aud.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
                    aud.Play();
                }
                if (useEffects && isPlayer)
                {
                    FindObjectOfType<EffectManager>().PlayEffect(onDeathEffect, transform.position);
                }
                cooldownStart = Time.time;
            }
        }
    }

    public void loadHealth()
    {
        ioFile io = new ioFile(healthFile);
        if (!io.exists())
        {
            saveHealth();
            return;
        }
        health = io.getitemvalueasfloat("health");
    }

    public void saveHealth()
    {
        ioFile io = new ioFile(healthFile);
        io.create();
        io.additem("health", health.ToString());
    }

}
