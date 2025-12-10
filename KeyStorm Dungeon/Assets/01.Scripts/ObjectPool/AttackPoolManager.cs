using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoolManager : MonoBehaviour
{
    [SerializeField] AttackObj attackObj;

    [SerializeField] int poolSize;

    Queue<AttackObj> queue = new Queue<AttackObj>();

    private void Awake()
    {
        queue = new Queue<AttackObj>();
    }

    private void Start()
    {
        InitPool();
    }

    void InitPool()
    {
        for(int i = 0; i < poolSize; i++)
        {
            CreatePool();
        }
    }

    void CreatePool()
    {
        AttackObj obj = Instantiate(attackObj, transform);
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
    }

    public AttackObj GetAttack()
    {
        if (queue.Count == 0)
            CreatePool();

        AttackObj obj = queue.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnPool(AttackObj obj)
    {
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
    }
}
