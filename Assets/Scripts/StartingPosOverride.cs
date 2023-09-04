using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPosOverride : MonoBehaviour
{

    private Vector3 desiredPosition;
    [SerializeField] private Transform spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        desiredPosition = spawnPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
