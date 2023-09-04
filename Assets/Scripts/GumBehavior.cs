using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumBehavior : MonoBehaviour
{
    public float lifeDuration = 2f;
    private float lifeTimer;
    public float speed = 8f;
    // Use this for initialization
    void Start()
    {
        lifeTimer = lifeDuration;
    }
    // Update is called once per frame
    void Update()
    {
        // Make the bullet move.
        transform.position += transform.forward * speed * Time.deltaTime;
        // Check if the bullet should be destroyed.
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != null)
        {
            Destroy(this.gameObject);
        }           
    }
}
