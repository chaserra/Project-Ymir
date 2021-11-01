using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPool : MonoBehaviour
{
    // Cache
    private List<ObjectPooler> objectPools = new List<ObjectPooler>();

    // Properties
    [SerializeField] GameObject[] effects;
    [SerializeField] int amountToPool = 3;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            objectPools.Add(new ObjectPooler(effects[i], amountToPool, gameObject));
        }
    }

    public GameObject GetPooledEffect(GameObject effect)
    {
        GameObject e;
        for (int i = 0; i < objectPools.Count; i++)
        {
            if (effect.GetInstanceID() == objectPools[i].InstanceID)
            {
                e = objectPools[i].GetPooledObject();
                return e;
            }
        }
        ObjectPooler p = new ObjectPooler(effect, amountToPool, gameObject);
        objectPools.Add(p);
        e = p.GetPooledObject();
        return e;
    }

}