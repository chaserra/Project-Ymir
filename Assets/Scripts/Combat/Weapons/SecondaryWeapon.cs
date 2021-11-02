using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SecondaryWeapon : Weapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void OnEnable()
    {
        hasHit = false;
        projectileBody.SetActive(true);
        distanceTravelled = 0f;
    }
}