using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class LightningPassiveSkill : PassiveSkill
{
    LightningPassiveData data;
    float damage;
    Vector2 detectSize;

    float hitTimer;
    int hitCount;
    bool isLightning;

    public LightningPassiveSkill(PlayerSkill playerSkill, LightningPassiveData data) : base(playerSkill, data)
    {
        this.data = data;
        this.damage = data.damage;
        detectSize = new Vector2(data.boxSize, data.boxSize);
    }

    protected override void Activate()
    {
        isLightning = true;
        hitCount = 0;
        hitTimer = 0f;
    }

    public override void DoPassive(float time)
    {
        if (!isLightning)
            return;

        hitTimer += time;
        if (hitTimer < data.hitInterval)
            return;

        hitTimer = 0f;
        Lightning();

        hitCount++;
        if (hitCount >= data.hitCount)
        {
            isLightning = false;
        }
    }

    void Lightning()
    {
        Collider2D[] enemyCols = Physics2D.OverlapBoxAll(Player.transform.position, detectSize, DefaultZero, data.enemyLayer);
        Monster monster = GetMonster(enemyCols);
        if(monster != null)
        {
            Lightning(monster.transform);
            monster.TakeDamage(damage);
        }
    }


    Monster GetMonster(Collider2D[] enemyCols)
    {
        if (enemyCols == null || enemyCols.Length == DefaultIntZero)
            return null;

        for (int i = 0; i < enemyCols.Length; i++)
        {
            int randIndex = Random.Range(DefaultIntZero, enemyCols.Length);
            Collider2D col = enemyCols[randIndex];

            if (col == null)
                continue;

            if (!col.gameObject.activeInHierarchy)
                continue;

            if (col.TryGetComponent(out Monster monster))
            {
                return monster;
            }
        }

        return null;
    }

    void Lightning(Transform target)
    {
        if (target == null)
            return;
        EffectPoolManager manager = Player.EffectPoolManager;
        Effect effect = manager.GetObj();
        effect.transform.position = target.position;
        effect.InitData(manager, data.effect, Vector2.zero, data.effectSize, target);
    }
}
