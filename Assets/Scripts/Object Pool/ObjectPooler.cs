using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    // Parameters
    GameObject objectToPool;
    int poolSize;

    // Attributes
    private int instanceID;
    public int InstanceID { get { return instanceID; } }

    // State
    private List<GameObject> objectList = new List<GameObject>();
    private GameObject parent;
    public GameObject ObjectParent { get { return parent; } }

    /// <summary>
    /// Pool to a specific parent
    /// </summary>
    public ObjectPooler(GameObject obj, GameObject p)
    {
        objectToPool = obj;
        parent = p;
        instanceID = objectToPool.gameObject.GetInstanceID();
    }

    /// <summary>
    /// Pool n times to a specific parent
    /// </summary>
    public ObjectPooler(GameObject obj, int size, GameObject p)
    {
        objectToPool = obj;
        poolSize = size;
        parent = p;
        instanceID = objectToPool.gameObject.GetInstanceID();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// Pool n times to a new GameObject as the parent
    /// </summary>
    public ObjectPooler(GameObject obj, int size, string name)
    {
        objectToPool = obj;
        poolSize = size;

        parent = new GameObject();
        parent.name = "POOL_" + name;

        instanceID = objectToPool.gameObject.GetInstanceID();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    public GameObject GetPooledObject()
    {
        if (objectList.Count <= 0) { return CreateNewObject(); }

        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            if (!objectList[i].activeSelf)
            {
                return objectList[i];
            }
        }
        return CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(objectToPool, parent.transform);
        objectList.Add(obj);
        obj.SetActive(false);
        return obj;
    }

    // Destroy specific object
    public void DestroyPooledObject(GameObject obj)
    {
        if (objectList.Count <= 0) { return; }
        int index = objectList.IndexOf(obj);
        Object.Destroy(objectList[index]);
        objectList.Remove(objectList[index]);
    }

    // Destroy last object
    public void DestroyLastPooledObject()
    {
        if (objectList.Count <= 0) { return; }
        GameObject lastObject = objectList[objectList.Count - 1];
        Object.Destroy(lastObject);
        objectList.RemoveAt(objectList.Count - 1);
    }

    // Destroy all objects
    public void DestroyAllPooledObjects()
    {
        if (objectList.Count <= 0) { return; }
        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            Object.Destroy(objectList[i]);
            objectList.Remove(objectList[i]);
        }
    }

}