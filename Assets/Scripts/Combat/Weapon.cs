using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] public int damage;
    [Header("Specs")]
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float projectileRange;

    public abstract void MoveProjectile();
    public abstract void DeactivateProjectile();
    public abstract void DeactivateObjectOnEffectEnd();
}