using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CarBehavior : MonoBehaviour
{
    enum AIState { patrol };

    private AIState currentState;
    private NavMeshAgent agent;

    [SerializeField] Transform[] navPoints;

    private GameObject player;
    private int currentPointIndex;



    // Start is called before the first frame update
    void Start()
    {
        currentState = AIState.patrol;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentPointIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleStates();
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
            default:
                Debug.Log("unknown state encountered for EnemyController/HandleStates");
                break;
        }
    }
}

