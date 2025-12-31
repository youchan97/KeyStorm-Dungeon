using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] Animator anim;

    EffectPoolManager poolManager;

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void InitData(EffectPoolManager pool, EffectData data, Vector2 dir)
    {
        poolManager = pool;
        anim.runtimeAnimatorController = data.animatorController;
        transform.right = dir;
        float duration = GetAnimationTime();
        CancelInvoke();
        Invoke(nameof(ReturnPool), duration);
    }

    float GetAnimationTime()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        AnimatorStateInfo animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.length;
    }

    void ReturnPool()
    {
        poolManager.ReturnPool(this);
    }
}
