using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Cache
    protected EffectsPool effectsPool;

    // Parameters
    [Header("General Damage")]
    [SerializeField] protected int damage;
    [Header("General Specs")]
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileRange;
    [Header("General Parts")]
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

    protected virtual void Update()
    {
        MoveProjectile();
        DeactivateObjectOnEffectEnd();
    }

}