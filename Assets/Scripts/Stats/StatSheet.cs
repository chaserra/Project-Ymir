using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSheet : ScriptableObject
{
    [Header("General Stats")]
    [SerializeField] private int health;
    public int Health { get { return health; } }
}