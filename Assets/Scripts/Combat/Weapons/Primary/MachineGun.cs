using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : PrimaryWeapon
{
    // Properties
    [Header("Hit Detection")]
    [SerializeField] protected float checkSphereSize = 7f;
    [SerializeField] protected float collisionSize = 2.5f;

    // Attributes
    private float raycastRange;
    private float forwardDetection;

    /* ** DEBUG ** */
    private void OnDrawGizmos()
    {
        // Display CheckSphere
        forwardDetection = checkSphereSize + .25f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * forwardDetection, checkSphereSize);
        // Display forward aim
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * forwardDetection);
        //Display hit sphere (only displays max range, assume capsule shape from start to max range)
        raycastRange = forwardDetection * 1.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * raycastRange, collisionSize);
    }

    protected override void OnEnable()
    {
        forwardDetection = checkSphereSize + .25f;
        raycastRange = forwardDetection * 1.5f;
        base.OnEnable();
    }

    protected override void Update()
    {
        DetectHit();
        base.Update();
    }

    protected override void DetectHit()
    {
        // Hit once
        if (hasHit) { return; }

        // Raycast only if something is detected around bullet
        if (Physics.CheckSphere(transform.position + transform.forward * forwardDetection, checkSphereSize))
        {
            if (Physics.SphereCast(transform.position, collisionSize, transform.forward, out RaycastHit hit, raycastRange))
            {
                if (hit.collider.CompareTag("Target"))
                {
                    // Trigger hit events
                    hit.collider.GetComponent<ITarget>().IsHit(damage);

                    // Set flag to prevent multiple hits
                    hasHit = true;

                    // Spawn pooled VFX
                    effectsPool.GetPooledEffect(hitVFX).GetComponent<VFX>().Play(hit.collider.transform.position);

                    // Set distance to disable when hitting the actual object
                    // Get distance between bullet and hit object
                    // Set distanceTravelled to remaining distance to hit object
                    float dist = Vector3.Distance(gameObject.transform.position, hit.collider.transform.position);
                    distanceTravelled = projectileRange - dist;
                }
            }
        }
    }
}
