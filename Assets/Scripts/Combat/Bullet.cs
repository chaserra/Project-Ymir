using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Cache
    private TrailRenderer bulletTrail;

    // Properties
    [Header("Damage")]
    [SerializeField] int damage = 1;
    [Header("Specs")]
    [SerializeField] float bulletSpeed = 500f;
    [SerializeField] float bulletRange = 750f;
    [SerializeField] float checkSphereSize = 6f;
    [SerializeField] float collisionSize = 3f;
    [Header("Parts")]
    [SerializeField] GameObject bulletBody;

    // Attributes
    private float raycastRange;
    private float forwardDetection;

    // State
    private float distanceTravelled = 0f;
    private bool hasHit = false;
    private bool hasStopped = false;
    private float disableTimer = 0f;

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

    private void Awake()
    {
        bulletTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        forwardDetection = checkSphereSize + .25f;
        raycastRange = forwardDetection * 1.5f;
        distanceTravelled = 0f;
        disableTimer = 0f;
        hasHit = false;
        hasStopped = false;
        bulletBody.SetActive(true);
        bulletTrail.emitting = true;
        bulletTrail.Clear();
    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        DetectHit();
        MoveBullet();
        DisableThisObjectOnTrailEnd();
    }

    private void DetectHit()
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

                    // Set distance to disable when hitting the actual object
                        // Get distance between bullet and hit object
                        // Set distanceTravelled to remaining distance to hit object
                    float dist = Vector3.Distance(gameObject.transform.position, hit.collider.transform.position);
                    distanceTravelled = bulletRange - dist;
                }
            }
        }
    }

    private void MoveBullet()
    {
        if (hasStopped) { return; }

        float distanceToTravel = bulletSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * distanceToTravel);
        distanceTravelled += distanceToTravel;

        if (distanceTravelled > bulletRange)
        {
            DeactivateBullet();
        }
    }

    private void DeactivateBullet()
    {
        // Deactivates body of the bullet and removes ability to hit objects
        // Also stops emitting bullet trail
        // The whole object is then deactivated upon end of bullet trail via DisableThisObjectOnTrailEnd()
        hasHit = true;
        hasStopped = true;
        bulletBody.SetActive(false);
        bulletTrail.emitting = false;
    }

    private void DisableThisObjectOnTrailEnd()
    {
        if (!hasStopped) { return; }
        // Disables the whole object once the trail has ended
        // Prevents VFX trails from disappearing on hit or upon reaching max distance
        // This also, in effect, returns this object to the object pool
        if (disableTimer <= bulletTrail.time)
        {
            disableTimer += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
