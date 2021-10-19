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

    // Attributes
    private GameObject poolParent;

    // State
    private float ignitionCounter = 0f;
    private float distanceTravelled = 0f;
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
        float distanceToTravel = missileSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * distanceToTravel);
        distanceTravelled += distanceToTravel;

        if (distanceTravelled > missileMaxRange)
        {
            gameObject.SetActive(false);
        }
    }

}
