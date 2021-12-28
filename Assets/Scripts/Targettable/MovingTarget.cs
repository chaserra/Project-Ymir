using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : Target
{
    private Vector3 lastPos;

    protected override void Awake()
    {
        base.Awake();
        lastPos = transform.position;
    }

    private void LateUpdate()
    {
        lastPos = transform.position;
    }

    public Vector3 GetVelocity()
    {
        Vector3 newPos = (transform.position - lastPos) / Time.deltaTime;
        return newPos;
    }

    public float GetSpeed()
    {
        float speed = GetVelocity().magnitude * Time.deltaTime;
        return speed;
    }

}