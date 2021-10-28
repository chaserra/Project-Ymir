using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Parameters
    [Header("Damage")]
    [SerializeField] int damage = 10;
    [SerializeField] float explosionRange = 3f;
    [Header("Specs")]
    [SerializeField] float ignitionTime = 1f;
    [SerializeField] float missileSpeed = 100f;
    [SerializeField] float missileMaxRange = 500f;
    [SerializeField] float turnSpeed = .6f;
    [SerializeField] GameObject explosion;
    [Header("Parts")]
    [SerializeField] GameObject missileBody;
    [SerializeField] ParticleSystem smokeTrail;

    // Attributes
    private GameObject poolParent;

    // State
    private float ignitionCounter = 0f;
    private float distanceTravelled = 0f;
    private bool hasHit = false;
    private GameObject target = null;

    private void Awake()
    {
        poolParent = transform.parent.gameObject;   // Set reference to the parent object pool
    }

    private void OnEnable()
    {
        hasHit = false;
        missileBody.SetActive(true);
        smokeTrail.Play();
        ignitionCounter = 0f;
        distanceTravelled = 0f;
        StartCoroutine(FireMissile());
    }

    private void OnDisable()
    {
        target = null;
    }

    private void Update()
    {
        MoveMissile();
        DisableThisObjectOnParticleEnd();
    }

    private IEnumerator FireMissile()
    {
        // Missile ignition. Missile fires/moves after the ignition.
        while (ignitionCounter < ignitionTime)
        {
            ignitionCounter += Time.deltaTime;
            yield return null;
        }
        // When activated, missile is parented to the shooter.
        // Code below reparents it to the object pool so the missile can move freely.
        transform.parent = poolParent.transform;
    }

    private void MoveMissile()
    {
        // Only move after ignition
        if (ignitionCounter < ignitionTime || hasHit) { return; }

        // Distance counter
        float distanceToTravel = missileSpeed * Time.deltaTime;

        // Move projectile
        transform.Translate(Vector3.forward * distanceToTravel);
        distanceTravelled += distanceToTravel;

        // Rotate projectile towards the target
        if (target != null)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        // Deactivate upon reaching max range
        if (distanceTravelled > missileMaxRange)
        {
            DeactivateMissile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) { return; }
        if (other.CompareTag("Target"))
        {
            other.GetComponent<ITarget>().IsHit(damage);
            Explode();
        }
    }

    private void Explode()
    {
        //TODO: Deal explosion proximity damage
        //TODO: Object pool
        Instantiate(explosion, transform.position, Quaternion.identity);
        DeactivateMissile();
    }

    private void DeactivateMissile()
    {
        // Deactivates body of the missile and removes ability to hit objects
        // Also stops emitting more smoke trails
        // The whole object is then deactivated upon end of all existing particles via DisableMissileOnParticleEnd()
        hasHit = true;
        missileBody.SetActive(false);
        smokeTrail.Stop();
    }

    private void DisableThisObjectOnParticleEnd()
    {
        // Disables the whole object once particles have ended playing
        // Prevents VFX trails from disappearing on hit or upon reaching max distance
        // This also, in effect, returns this object to the object pool
        if (!smokeTrail.IsAlive())
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }

}
