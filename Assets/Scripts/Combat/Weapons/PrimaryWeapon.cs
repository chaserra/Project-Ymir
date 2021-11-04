using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrimaryWeapon : Weapon
{
    // Cache
    protected TrailRenderer particleTrail;

    // State
    protected bool hasStopped = false;
    protected float disableTimer = 0f;

    protected abstract void DetectHit();    // Special DetectHit is used since PrimaryWeapon projectiles are too fast for OnTriggerEnter

    protected override void Awake()
    {
        base.Awake();
        particleTrail = GetComponentInChildren<TrailRenderer>();
    }

    protected virtual void OnEnable()
    {
        distanceTravelled = 0f;
        disableTimer = 0f;
        hasHit = false;
        hasStopped = false;
        projectileBody.SetActive(true);
        particleTrail.emitting = true;
        particleTrail.Clear();
    }

    protected override void MoveProjectile()
    {
        if (hasStopped) { return; }

        float distanceToTravel = projectileSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * distanceToTravel);
        distanceTravelled += distanceToTravel;

        if (distanceTravelled > projectileRange)
        {
            DeactivateProjectile();
        }
    }

    protected override void DeactivateProjectile()
    {
        // Deactivates body of the bullet and removes ability to hit objects
        // Also stops emitting bullet trail
        // The whole object is then deactivated upon end of bullet trail via DisableThisObjectOnTrailEnd()
        hasHit = true;
        hasStopped = true;
        projectileBody.SetActive(false);
        particleTrail.emitting = false;
    }

    protected override void DeactivateObjectOnEffectEnd()
    {
        if (!hasStopped) { return; }
        // Disables the whole object once the trail has ended
        // Prevents VFX trails from disappearing on hit or upon reaching max distance
        // This also, in effect, returns this object to the object pool
        if (disableTimer <= particleTrail.time)
        {
            disableTimer += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
