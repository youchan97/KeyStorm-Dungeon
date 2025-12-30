using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackSkill : ISkill
{
    Player player;
    KnockBackData data;

    public SkillType SkillType => data.skillType;

    public bool IsFinish {  get; private set; }

    public KnockBackSkill(PlayerSkill playerSkill, KnockBackData data)
    {
        this.player = playerSkill.player;
        this.data = data;
    }

    public void Enter()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(
            player.transform.position,
            data.radius
        );

        foreach (var col in cols)
        {
            Monster monster = col.GetComponent<Monster>();

            if (monster == null) continue;

            if (monster.MonsterRb != null)
            {
                Vector2 dir = monster.transform.position - player.transform.position;
                monster.ApplyKnockBack(dir, data.force, data.duration);
                //monster.MonsterRb.AddForce(dir.normalized * data.force, ForceMode2D.Impulse);
            }
        }

        IsFinish = true;
    }


    public void Exit()
    {
    }

    public void Update()
    {
    }

}
