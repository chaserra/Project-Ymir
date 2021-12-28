using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Target : MonoBehaviour, ITarget
{
    // Cache
    protected TargetBounds targetBounds;
    protected Camera cam;
    protected EffectsPool effectsPool;

    // Parameters
    [SerializeField] protected StatSheet targetStats;
    [SerializeField] protected int health;    // TODO: Remove SerializeField
    [SerializeField] protected GameObject explosionVFX;

    // Attributes
    public StatSheet TargetStats { get { return targetStats; } }

    protected virtual void Awake()
    {
        //health = GetComponent<Ship>().shipStats.Health;
        targetBounds = new TargetBounds(GetComponentInChildren<Renderer>().bounds);
        cam = Camera.main;
        effectsPool = FindObjectOfType<EffectsPool>();
        health = targetStats.Health;
        if (explosionVFX == null) { Debug.LogError("No VFX added!"); }
    }

    public GameObject ThisGameObject()
    {
        return gameObject;
    }

    public void IsHit(int damage)
    {
        health -= damage;
        Die();
    }

    private void Die()
    {
        if (health <= 0)
        {
            Debug.Log(name + " destroyed!");
            NoLongerTargetted();
            effectsPool.GetPooledEffect(explosionVFX).GetComponent<VFX>().Play(transform.position);
            Destroy(gameObject);
        }
    }

    public Rect GetTargetScreenPos(GameObject source)
    {
        WeaponsController wc;
        Camera newCam = cam; // Failsafe

        if (source.TryGetComponent(out wc))
        {
            newCam = wc.Camera;
        }
        return targetBounds.FocusOnBounds(newCam);
    }

    public void NoLongerTargetted()
    {
        // Trigger a bool of some sort. Maybe use this for anti-homing stuff
    }

    public int GetHealth()
    {
        return health;
    }
}
