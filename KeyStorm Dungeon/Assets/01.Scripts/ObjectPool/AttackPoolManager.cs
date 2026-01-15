using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoolManager : PoolManager<AttackObj>
{
    public EffectPoolManager effectPoolManager;

    public override AttackObj GetObj()
    {
        AttackObj obj = null;

        for (int i = 0; i < queue.Count; i++)
        {
            AttackObj currentObj = queue.Dequeue();

            if (!currentObj.IsActive)
            {
                obj = currentObj;
                break;
            }
            else
            {
                queue.Enqueue(currentObj);
            }
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public override void ReturnPool(AttackObj obj)
    {
        obj.ResetObj();
        base.ReturnPool(obj);
    }
}
