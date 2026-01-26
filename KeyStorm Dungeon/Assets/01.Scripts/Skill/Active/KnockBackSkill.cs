using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackSkill : IActiveSKill
{
    KnockBackData data;

    public bool IsFinish {  get; private set; }

    public Player Player { get; private set; }

    public KnockBackSkill(PlayerSkill playerSkill, KnockBackData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }

    public void Enter()
    {
        IsFinish = false;
        Collider2D[] cols = Physics2D.OverlapCircleAll(
            Player.transform.position,
            data.radius
        );

        foreach (var col in cols)
        {
            Monster monster = col.GetComponent<Monster>();

            if (monster == null) continue;

            if (monster.MonsterRb != null)
            {
                Vector2 dir = monster.transform.position - Player.transform.position;
                monster.ApplyKnockBack(dir, data.force, data.duration);
                //monster.MonsterRb.AddForce(dir.normalized * data.force, ForceMode2D.Impulse);
            }
        }
        KnockBackEffect();
        IsFinish = true;
    }


    public void Exit()
    {
    }

    public void Update()
    {
    }

    void KnockBackEffect()
    {
        EffectPoolManager manager = Player.EffectPoolManager;
        Effect effect = manager.GetObj();
        Transform target = Player.transform;
        effect.transform.position = target.position;
        effect.InitData(manager, data.effect, Vector2.zero, data.effectSize, target);
    }
}
