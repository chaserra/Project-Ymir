using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : Target
{
    private float currentForwardSpeed = 0f;
    // Setter
    public float SetForwardSpeed { set { currentForwardSpeed = value; } }
    // Getter
    public float CurrentForwardSpeed { get { return currentForwardSpeed * Time.fixedDeltaTime; } }

}