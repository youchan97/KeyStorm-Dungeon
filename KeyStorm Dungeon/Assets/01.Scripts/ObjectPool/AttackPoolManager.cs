using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoolManager : PoolManager<AttackObj>
{
    public EffectPoolManager effectPoolManager;

    public override void ReturnPool(AttackObj obj)
    {
        obj.ResetObj();
        base.ReturnPool(obj);
    }
}
