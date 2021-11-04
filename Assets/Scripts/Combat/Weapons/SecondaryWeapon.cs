using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SecondaryWeapon : Weapon
{
    // Parameters
    [Header("Secondary Specs")]
    [SerializeField] protected float ignitionTime = 1f;

    // State
    protected float ignitionCounter = 0f;
    protected GameObject target = null;

    protected virtual void OnEnable()
    {
        hasHit = false;
        projectileBody.SetActive(true);
        distanceTravelled = 0f;
        ignitionCounter = 0f;
    }

    protected virtual IEnumerator IgnitionPhase()
    {
        // Initial phase for secondary weapons. This acts as a charge up to when the shot fires.
        while (ignitionCounter < ignitionTime)
        {
            ignitionCounter += Time.deltaTime;
            yield return null;
        }
    }

    protected override void DeactivateProjectile()
    {
        // Deactivates body of the missile and removes ability to hit objects
        // Also stops emitting more smoke trails
        // The whole object is then deactivated upon end of all existing particles via DisableMissileOnParticleEnd()
        hasHit = true;
        projectileBody.SetActive(false);
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }

}