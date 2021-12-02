using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret Stats", menuName = "Stat Sheets/Turret Stat")]
public class TurretStats : StatSheet
{
    [Header("Controls")]
    [SerializeField] private float rotateSpeed;

    public float YawSpeed { get { return rotateSpeed; } }

}
