using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public string targetTag = "Player";
    [Space(10)]
    public bool onlyChase = false;
    public AIPoints aiPoints;
    public float maxWait = 5;
    [Space(10)]
    public bool disableEye = false;
    public Transform eye;
    public float eyeLookOffsetYNeg = 1;
    public float chaseSpeedMult = 5f;
    public float chaseDistance = 3.5f;
    public bool enableChaseSound = false;
    public AudioSource chaseSound;
    [Space(10)]
    public bool enableDamage = false;
    public GameObject damage;
    public float attackRange = 1.4f;
    [Space(10)]
    public bool enableAnimation = false;
    public Animator anim;
    public string moveBool = "Move";
    public string chaseBool = "Chase";
    public string[] attackTriggers;
    public float attackInterval = 1;

    private NavMeshAgent agent;
    private float lastWait = 0;
    private float waitTime = 0;
    private float lastAttack = 0;
    private bool chase = false;
    private GameObject target;
    private bool waiting = false;
    private int lastPoint = -1;
    private Vector3 lastPosition;

    private void Start()
    {
        if (aiPoints == null)
            aiPoints = FindObjectOfType<AIPoints>();
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (!agent.enabled)
        {
            this.enabled = false;
        }
        if (chase)
        {
            Debug.Log("CHASING OVER DIST: " + agent.remainingDistance);
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
                if (Vector3.Magnitude(target.transform.position - transform.position) > chaseDistance)
                {
                    if (!Look(target.transform.position, target))
                    {
                        LoseTarget();
                        Debug.Log("stopped chasing");
                    }
                    return;
                }
                if (enableDamage)
                {
                    if (Vector3.Magnitude(target.transform.position - transform.position) <= attackRange)
                    {
                        damage.SetActive(true);
                    }
                    else
                    {
                        damage.SetActive(false);
                    }
                }
                if (enableAnimation)
                {
                    if (agent.remainingDistance <= 3f)
                    {
                        if (Time.time - lastAttack >= attackInterval)
                        {
                            lastAttack = Time.time;
                            anim.SetTrigger(attackTriggers[UnityEngine.Random.Range(0, attackTriggers.Length)]);
                            Debug.Log("attacked");
                        }
                    }
                }
            }
            else
            {
                LoseTarget();
                Debug.Log("stopped chasing");
            }
        }
        else
        {
            if (!onlyChase)
            {
                if (!waiting && (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0 || !agent.hasPath) || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    //Debug.Log("not waiting xxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    //if (enableAnimation)
                        //anim.SetBool(moveBool, false);
                    agent.isStopped = true;
                    agent.ResetPath();
                    waitTime = UnityEngine.Random.Range(maxWait / 2, maxWait);
                    lastWait = Time.time;
                    waiting = true;
                    Debug.Log("started waiting (" + waitTime + ")");
                }
                else
                {
                    if (Time.time - lastWait >= waitTime && waiting && (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0 || !agent.hasPath))
                    {
                        //Debug.Log("waiting xxxxxxxxxxxxxxxxxxxxxxxxxxx");
                        //if (enableAnimation)
                            //anim.SetBool(moveBool, true);
                        int rand = -1;
                        rand = UnityEngine.Random.Range(0, aiPoints.points.Length);
                        /*for (; ; )
                        {
                            rand = UnityEngine.Random.Range(0, aiPoints.points.Length);
                            if (rand != lastPoint)
                                break;
                        }*/
                        //Debug.Log("RAND: "+rand);
                        agent.SetDestination(aiPoints.points[rand].position);
                        lastPoint = rand;
                        agent.isStopped = false;
                        waiting = false;
                        Debug.Log("finished waiting");
                    }
                }
            }
        }/*
        if ((waiting || !chase) && (!waiting && !chase))
        {
            if (enableAnimation)
                anim.SetBool(moveBool, false);
        }
        else
        {
            if (enableAnimation)
                anim.SetBool(moveBool, true);
        }
        */
        if (lastPosition != transform.position)
        {
            if (enableAnimation)
                anim.SetBool(moveBool, true);
            lastPosition = transform.position;
        }
        else
        {
            if (enableAnimation)
                anim.SetBool(moveBool, false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (!chase)
            {
                if (Look(other.transform.position, other.gameObject))
                {
                    agent.speed *= chaseSpeedMult;
                    target = other.gameObject;
                    chase = true;
                    if (enableChaseSound)
                    {
                        chaseSound.Play();
                    }
                    waiting = false;
                    if (enableAnimation)
                    {
                        anim.SetBool(moveBool, true);
                        anim.SetBool(chaseBool, true);
                    }
                    Debug.Log("started chasing");
                }
            }
        }
    }

    private void LoseTarget()
    {
        if (enableAnimation)
        {
            anim.SetBool(chaseBool, false);
        }
        agent.ResetPath();
        if (enableChaseSound)
        {
            chaseSound.Stop();
        }
        agent.speed /= chaseSpeedMult;
        chase = false;
        target = null;
    }

    private bool Look(Vector3 pos, GameObject obj)
    {
        if (disableEye)
        {
            return true;
        }
        GameObject objx = null;
        if (obj.GetComponent<PlayerController>() != null)
        {
            objx = obj.GetComponent<PlayerController>().look.gameObject;
            pos = objx.transform.position;
        }
        pos = new Vector3(pos.x, pos.y - eyeLookOffsetYNeg, pos.z);
        eye.LookAt(pos);
        RaycastHit hit = new RaycastHit();
        int layermask = 1 << 4;
        layermask = ~layermask;
        if (Physics.Raycast(eye.position, eye.forward, out hit, 100, layermask, QueryTriggerInteraction.UseGlobal))
        {
            return hit.collider.gameObject == obj;
        }
        else
        {
            return false;
        }
    }

}
