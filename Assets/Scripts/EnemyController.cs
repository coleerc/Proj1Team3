using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum AIState { patrol, chase, stunned };

    private AIState currentState;
    private NavMeshAgent agent;

    [SerializeField] Transform[] navPoints;

    private GameObject player;
    private int currentPointIndex;
    private float distancePlayer;
    private bool isStunned;
    private float stunnedDuration;



    // Start is called before the first frame update
    void Start()
    {
        stunnedDuration = 0;
        currentState = AIState.patrol;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentPointIndex = 0;
        isStunned= false;
    }

    // Update is called once per frame
    void Update()
    {
        DetermineStates();
        HandleStates();
        if(stunnedDuration > 0)
        {
            stunnedDuration -= Time.deltaTime;
        }
        else
        {
            stunnedDuration= 0;
            isStunned = false;
        }
    }

    private void DetermineStates()
    {
        distancePlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distancePlayer <= 8.0 && !isStunned)
        {
            currentState = AIState.chase;
            agent.speed = 3.8f;
            agent.acceleration = 8.0f;
            //print("chasing");
        }
        else if (!isStunned)
        {
            currentState = AIState.patrol;
            agent.speed = 2.0f;
            agent.acceleration = 8.0f;
            //print("patrolling");
        }
        else if (isStunned)
        {
            currentState = AIState.stunned;
            //print("stunned");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "gumprojectile")
        {
            print("hit enemy");
            isStunned = true;
            stunnedDuration += 5.0f;
        }
    }
    private void HandleStates()
    {
        switch (currentState)
        {
            case AIState.patrol:
                if (!agent.pathPending && agent.remainingDistance <= 0.5f)
                {
                    agent.SetDestination(navPoints[currentPointIndex].position);
                    currentPointIndex++;
                    if (currentPointIndex >= navPoints.Length)
                    {
                        currentPointIndex = 0;
                    }
                }
                break;
            case AIState.chase:
                //set destination to player pos
                agent.SetDestination(player.transform.position);
                break;
            case AIState.stunned:
                agent.speed = 0.0f;
                agent.acceleration = 0.0f;
                break;
            default:
                Debug.Log("unknown state encountered for EnemyController/HandleStates");
                break;
        }
    }
}

