using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, ITarget
{
    // Cache
    private TargetBounds targetBounds;
    private Camera cam;
    private EffectsPool effectsPool;

    // Parameters
    [SerializeField] private int health;    // TODO: Remove SerializeField
    [SerializeField] GameObject explosionVFX;

    private void Awake()
    {
        //health = GetComponent<Ship>().shipStats.Health;
        targetBounds = new TargetBounds(GetComponentInChildren<Renderer>().bounds);
        cam = Camera.main;
        effectsPool = FindObjectOfType<EffectsPool>();
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
