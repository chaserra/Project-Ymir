using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, ITarget
{
    // Cache
    private TargetBounds targetBounds;
    private Camera cam;

    // Parameters
    [SerializeField] int health = 100;

    private void Awake()
    {
        targetBounds = new TargetBounds(GetComponentInChildren<Renderer>().bounds);
        cam = Camera.main;
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
}
