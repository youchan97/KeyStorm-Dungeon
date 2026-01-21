using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class Effect : MonoBehaviour
{
    private readonly Vector2 defaultSize = new Vector2(DefaultEffectSize, DefaultEffectSize);

    [SerializeField] Animator anim;
    Transform target;

    EffectPoolManager poolManager;

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            transform.position = target.position;
        }
    }

    public void InitData(EffectPoolManager pool, EffectData data, Vector2 dir, float size = DefaultEffectSize, Transform target = null)
    {
        poolManager = pool;
        anim.runtimeAnimatorController = data.animatorController;
        transform.right = dir;
        transform.localScale *= size;
        this.target = target;
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

    void ResetEffect()
    {
        transform.localScale = defaultSize;
    }

    void ReturnPool()
    {
        ResetEffect();
        poolManager.ReturnPool(this);
    }
}
