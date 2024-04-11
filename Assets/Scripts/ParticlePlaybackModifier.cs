using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlaybackModifier : MonoBehaviour
{
    ParticleSystem particle;
    public float playbackSpeed;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        var main = particle.main;
        main.simulationSpeed = playbackSpeed;
    }
}
