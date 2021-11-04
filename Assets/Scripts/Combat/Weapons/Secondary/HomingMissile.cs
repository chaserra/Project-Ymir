using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : SecondaryWeapon
{
    // Parameters
    [Header("Specs")]
    [SerializeField] float explosionRange = 23f;
    [SerializeField] float turnSpeed = .6f;
    [Header("Parts")]
    [SerializeField] ParticleSystem smokeTrail;

    // Attributes
    private GameObject poolParent;

    protected override void Awake()
    {
        poolParent = transform.parent.gameObject;   // Set reference to the parent object pool
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        smokeTrail.Play();
        StartCoroutine(IgnitionPhase());
    }

    private void OnDisable()
    {
        target = null;
    }

    /* ** DEBUG ** */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    protected override IEnumerator IgnitionPhase()
    {
        // Missile ignition. Missile fires/moves after the ignition.
        yield return base.IgnitionPhase();

        // When activated, missile is parented to the shooter.
        // Code below reparents it to the object pool so the missile can move freely.
        transform.parent = poolParent.transform;
    }

    protected override void MoveProjectile()
    {
        // Only move after ignition
        if (ignitionCounter < ignitionTime || hasHit) { return; }

        // Distance counter
        float distanceToTravel = projectileSpeed * Time.deltaTime;

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
        if (distanceTravelled > projectileRange)
        {
            DeactivateProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) { return; }
        if (other.CompareTag("Target"))
        {
            other.GetComponent<ITarget>().IsHit(damage);
            Explode(other);
        }
    }

    private void Explode(Collider firstHit)
    {
        // Spawn pooled VFX
        effectsPool.GetPooledEffect(hitVFX).GetComponent<VFX>().Play(transform.position);

        // Deactivate missile (prevent multi hits)
        DeactivateProjectile();

        // Explosion damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject == firstHit.gameObject) { return; } // Ignore source
            if (colliders[i].CompareTag("Target"))
            {
                colliders[i].GetComponent<ITarget>().IsHit(damage);
            }
        }
    }

    protected override void DeactivateProjectile()
    {
        base.DeactivateProjectile();
        smokeTrail.Stop();
    }

    protected override void DeactivateObjectOnEffectEnd()
    {
        // Disables the whole object once particles have ended playing
        // Prevents VFX trails from disappearing on hit or upon reaching max distance
        // This also, in effect, returns this object to the object pool
        if (!smokeTrail.IsAlive())
        {
            gameObject.SetActive(false);
        }
    }

}
