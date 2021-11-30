using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Stats", menuName = "Ship Stat")]
public class ShipStats : ScriptableObject
{
    [Header("Speed")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [Header("Controls")]
    [SerializeField] private float yawSpeed;
    [SerializeField] private float pitchSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float thrusterSpeed;

    public float MinSpeed { get { return minSpeed; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float YawSpeed { get { return yawSpeed; } }
    public float PitchSpeed { get { return pitchSpeed; } }
    public float RollSpeed { get { return rollSpeed; } }
    public float ThrusterSpeed { get { return thrusterSpeed; } }

}
