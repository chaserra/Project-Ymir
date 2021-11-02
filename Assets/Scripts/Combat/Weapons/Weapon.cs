using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Cache
    protected EffectsPool effectsPool;

    // Parameters
    [Header("Damage")]
    [SerializeField] protected int damage;
    [Header("Specs")]
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileRange;
    [Header("Parts")]
    [SerializeField] protected GameObject projectileBody;
    [SerializeField] protected GameObject hitVFX;

    // State
    protected float distanceTravelled = 0f;
    protected bool hasHit = false;

    protected abstract void MoveProjectile();
    protected abstract void DeactivateProjectile();
    protected abstract void DeactivateObjectOnEffectEnd();

    protected virtual void Awake()
    {
        effectsPool = FindObjectOfType<EffectsPool>();
    }
}