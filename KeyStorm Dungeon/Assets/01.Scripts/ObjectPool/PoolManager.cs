using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] T poolObj;

    [SerializeField] int poolSize;

    Queue<T> queue = new Queue<T>();

    private void Start()
    {
        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreatePool();
        }
    }

    void CreatePool()
    {
        T obj = Instantiate(poolObj, transform);
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
    }

    public virtual T GetObj()
    {
        if (queue.Count == 0)
            CreatePool();

        T obj = queue.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public virtual void ReturnPool(T obj)
    {
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
    }
}
