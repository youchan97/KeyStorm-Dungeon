using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolData
    {
        public string poolName;
        public GameObject prefab;
        public int initialSize;
    }

    private static ObjectPoolManager instance;

    public static ObjectPoolManager Instance { get { return instance; } }

    [SerializeField] private List<PoolData> pools = new List<PoolData>();

    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (PoolData data in pools)
        {
            if (data.prefab == null)
            {
                Debug.LogWarning($"ObjectPoolManager: {data.poolName} 풀의 프리팹이 할당되지 않음");
                continue;
            }

            Queue<GameObject> poolQueue = new Queue<GameObject>();

            for (int i = 0; i < data.initialSize; i++)
            {
                GameObject obj = Instantiate(data.prefab, transform);
                obj.SetActive(false);
                poolQueue.Enqueue(obj);
            }
            objectPools.Add(data.poolName, poolQueue);
        }
    }

    public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!objectPools.ContainsKey(poolName))
        {
            return null;
        }

        Queue<GameObject> poolQueue = objectPools[poolName];
        GameObject obj;

        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            {
                PoolData data = pools.Find(p => p.poolName == poolName);

                if (data.prefab == null) return null;

                obj = Instantiate(data.prefab, transform);
            }
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj, string poolName = "")
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (!string.IsNullOrEmpty(poolName) && objectPools.ContainsKey(poolName))
        {
            objectPools[poolName].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    public T GetObject<T>(string poolName, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = GetObject(poolName, position, rotation);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }
}
