using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class LeadingAI : MonoBehaviour
{

    public Transform destination;
    public float delay = 5;
    public Transform player;

    private NavMeshAgent agent;
    private float startTime;
    private bool moving = false;

    private void Start()
    {
        transform.position = player.position;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        startTime = Time.time;
        Debug.Log("Leading AI --- Appeared");
    }

    private void Update()
    {
        if (Time.time - startTime >= delay && !moving)
        {
            agent.isStopped = false;
            agent.SetDestination(destination.position);
            moving = true;
            Debug.Log("Leading AI --- Moving");
        }
    }

}
