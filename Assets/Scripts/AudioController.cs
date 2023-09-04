using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioController : MonoBehaviour
{
    public AudioSource footsteps;
    public AudioSource ghostTouched;
    public AudioSource collectedCandy;
    public AudioSource heartbeat;
    [SerializeField] private PlayerBehaviour player; // <- change to be wherever your isWalking bool is
    [SerializeField] private AudioClip footstepSound; // Assign the footstep sound in the Unity Inspector
    [SerializeField] private AudioClip heartbeatSound;
    [SerializeField] private float stepFrequency = 1f; // Time between each footstep, adjustable in Inspector


    void Start()
    {
        footsteps = GetComponent<AudioSource>();
        if (footsteps == null)
        {
            Debug.LogWarning("No AudioSource component found on this GameObject. Footstep sounds will not play.");
            return;
        }

        StartCoroutine(PlayFootstepSounds());
        StartCoroutine(PlayHeartbeatSounds());
    }

    IEnumerator PlayFootstepSounds()
    {
        while (true)
        {
            if (player.isWalking)  // Check the walking state dynamically
            {
                footsteps.PlayOneShot(footstepSound);
                Debug.Log("step");
                yield return new WaitForSeconds(stepFrequency);
            }
            else
            {
                yield return null; // Wait for next frame to re-check
            }
        }
    }

    IEnumerator PlayHeartbeatSounds()
    {
        while (true)
        {
            if (player.spirit <= 20)  // Check the spirit state dynamically
            {
                heartbeat.PlayOneShot(heartbeatSound);
                Debug.Log("heartbeat");
                yield return new WaitForSeconds(stepFrequency);
            }
            else
            {
                yield return null; // Wait for next frame to re-check
            }
        }
    }
    public void PlayCrinkle()
    {
        collectedCandy.Play();
    }

    public void PlayGhostTouched()
    {
        ghostTouched.Play();
    }
}
