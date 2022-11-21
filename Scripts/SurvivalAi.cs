using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum SurvivalAiStates
{
    idle,
    seek,
    chase
}

[RequireComponent(typeof(NavMeshAgent))]
public class SurvivalAi : MonoBehaviour
{
    [Header("Survival AI")]
    public bool enabled = false;
    public bool enableDebugging = false;
    public TransformArray points;
    [Space(10)]
    [Header("Behaviour")]
    public bool allowIdle = false;
    public float minIdleDuration = 5f;
    public float maxIdleDuration = 20f;
    [Space(5)]
    public bool allowSeek = false;
    public float seekArrivalDistance = 0.05f;
    [Space(5)]
    public bool allowChase = false;
    [Space(10)]
    [Header("Interaction")]
    public bool enableInteraction = false;
    public Transform eyeInteraction;
    public float interactionDistance = 3;
    public bool autoOpenDoors = false;
    public float interactionInterval = 0.1f;
    [Space(10)]
    [Header("Animation")]
    public bool enableAnim = false;
    public Animator anim;
    public string moving = "Moving";
    public string running = "Running";
    public float animInterval = 0.1f;

    private NavMeshAgent agent;

    private SurvivalAiStates state = SurvivalAiStates.idle;
    private float lastIdleTime = 0;
    private bool stateStarted = false;
    private bool stateFinished = false;
    private float rndIdleDuration = 0;
    private Transform seekTarget;

    private float lastInteractionTime = 0;

    private Vector3 lastPosition = new Vector3(0, 0, 0);
    private float lastTimeAnim = 0;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        getNewState("start");
    }

    private void Update()
    {
        if (enabled)
        {
            // animation
            if (enableAnim)
            {
                // ---------- add running code
                anim.SetBool(moving, lastPosition != transform.position);
                lastPosition = transform.position;
            }

            // behaviour
            if (!stateFinished)
            {
                switch (state)
                {
                    case SurvivalAiStates.idle:
                        if (allowIdle)
                        {
                            if (!stateStarted)
                            {
                                if (!agent.isStopped)
                                    agent.isStopped = true;
                                rndIdleDuration = Random.Range(minIdleDuration, maxIdleDuration);
                                lastIdleTime = Time.time;
                                stateStarted = true;
                            }
                            else
                            {
                                if (Time.time - lastIdleTime >= rndIdleDuration)
                                {
                                    stateFinished = true;
                                    getNewState("*idle");
                                }
                            }
                        }
                        else
                            getNewState("!idle");
                        break;
                    case SurvivalAiStates.seek:
                        if (allowSeek)
                        {
                            if (!stateStarted)
                            {/*
                                if (seekTarget == null)
                                {
                                    seekTarget = points.getRandom();
                                }
                                Transform t = seekTarget;
                                for (; ; )
                                {
                                    seekTarget = points.getRandomSpecial(t);
                                    agent.SetDestination(seekTarget.position);
                                    if (agent.pathStatus != NavMeshPathStatus.PathInvalid && agent.pathStatus != NavMeshPathStatus.PathPartial)
                                    {
                                        break;
                                    }
                                }
                                */
                                seekTarget = points.getRandom();
                                agent.SetDestination(seekTarget.position);
                                agent.isStopped = false;
                                stateStarted = true;
                            }
                            else
                            {
                                if (agent.remainingDistance <= agent.stoppingDistance + seekArrivalDistance)
                                {
                                    stateFinished = true;
                                    getNewState("*seek @ " + seekTarget.name + "; dist = " + agent.remainingDistance);
                                }
                                else
                                {
                                    if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
                                    {
                                        getNewState("?seek @ " + seekTarget.name);
                                    }
                                }
                            }
                        }
                        else
                            getNewState("!seek");
                        break;
                    case SurvivalAiStates.chase:

                        break;
                }
            }

            // interaction
            if (enableInteraction)
            {
                if (Time.time - lastInteractionTime >= interactionInterval)
                {
                    lastInteractionTime = Time.time;
                    if (autoOpenDoors)
                    {
                        RaycastHit hit;
                        int layerMask = ~LayerMask.GetMask("Player");
                        if (Physics.Raycast(eyeInteraction.position, eyeInteraction.forward, out hit, interactionDistance, layerMask, QueryTriggerInteraction.UseGlobal))
                        {
                            GameObject obj = hit.collider.gameObject;
                            if (obj.GetComponent<DoorManager>() != null)
                            {
                                DoorManager door = obj.GetComponent<DoorManager>();
                                if (!door.isOpen())
                                {
                                    if (!door.isLocked())
                                    {
                                        door.Open();
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    private void getNewState(string sender)
    {
        stateStarted = false;
        stateFinished = false;
        SurvivalAiStates s = SurvivalAiStates.idle;
        int i = Random.Range(state == SurvivalAiStates.idle ? 1 : 0, 2);
        switch (i)
        {
            case 0:
                s =  SurvivalAiStates.idle;
                break;
            case 1:
                s = SurvivalAiStates.seek;
                break;
        }
        if ((!allowIdle && s == SurvivalAiStates.idle) || (!allowSeek && s == SurvivalAiStates.seek))
        {
            getNewState("new");
        }
        else
        {
            state = s;
            if (enableDebugging)
                Debug.Log(sender + " - state set = " + state.ToString());
        }
    }
}
