using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private GlobalClock clock;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clock = GameObject.FindGameObjectWithTag("GlobalClock").GetComponent<GlobalClock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (clock.isSimulationPlaying)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
