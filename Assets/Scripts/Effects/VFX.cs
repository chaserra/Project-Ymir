using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    private ParticleSystem[] particles;

    private void Awake()
    {
        particles = new ParticleSystem[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf) { return; }

        // Deactivate object if all particles are no longer playing
        if (!ParticleIsPlaying())
        {
            gameObject.SetActive(false);
        }
    }

    // Play vfx on position
    public void Play(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
        }
    }

    // Particle state checker
    private bool ParticleIsPlaying()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].IsAlive())
            {
                return true;
            }
        }
        return false;
    }

}