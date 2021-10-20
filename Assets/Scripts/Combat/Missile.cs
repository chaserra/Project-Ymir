using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Parameters
    [SerializeField] int damage = 10;
    [SerializeField] float explosionRange = 3f;
    [SerializeField] float ignitionTime = 1f;
    [SerializeField] float missileSpeed = 100f;
    [SerializeField] float missileMaxRange = 500f;
    [SerializeField] float turnSpeed = 10f;

    // Attributes
    private GameObject poolParent;

    // State
    private float ignitionCounter = 0f;
    private float distanceTravelled = 0f;
    private GameObject target = null;
    // TODO: Do a more optimized version of this
    public GameObject Target { set { target = value; } }
    private bool hasHit = false;

    private void Awake()
    {
        poolParent = transform.parent.gameObject;
    }

    private void OnEnable()
    {
        ignitionCounter = 0f;
        distanceTravelled = 0f;
        hasHit = false;
        StartCoroutine(FireMissile());
    }

    private void OnDisable()
    {
        target = null;
    }

    private void Update()
    {
        MoveMissile();
    }

    private IEnumerator FireMissile()
    {
        while (ignitionCounter < ignitionTime)
        {
            ignitionCounter += Time.deltaTime;
            yield return null;
        }
        transform.parent = poolParent.transform;
    }

    private void MoveMissile()
    {
        if (ignitionCounter < ignitionTime) { return; }
        // Distance counter
        float distanceToTravel = missileSpeed * Time.deltaTime;

        // Move projectile
        transform.Translate(Vector3.forward * distanceToTravel);
        distanceTravelled += distanceToTravel;

        // Rotate projectile
        if (target != null)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        // Deactivate upon reaching max range
        if (distanceTravelled > missileMaxRange)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) { return; }
        if (other.CompareTag("Target"))
        {
            hasHit = true;
            //TODO: Deal explosion proximity damage
            other.GetComponent<ITarget>().IsHit(damage);
            gameObject.SetActive(false);
        }
    }

}
