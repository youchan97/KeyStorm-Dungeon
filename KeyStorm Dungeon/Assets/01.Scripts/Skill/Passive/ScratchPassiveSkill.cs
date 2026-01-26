using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class ScratchPassiveSkill : PassiveSkill
{
    ScratchPassiveData data;
    float damage;
    Vector2 detectSize;

    float hitTimer;
    int hitCount;
    bool isScratching;

    Monster targetMonster;

    public ScratchPassiveSkill(PlayerSkill playerSkill, ScratchPassiveData data) : base(playerSkill, data)
    {
        this.data = data;
        detectSize = new Vector2(data.boxSize, data.boxSize);
    }

    protected override void Activate()
    {
        isScratching = true;
        hitCount = DefaultIntOne;
        hitTimer = DefaultZero;
        damage = Player.PlayerAttack.Damage * Player.PlayerAttack.DamageMultiple * data.damageMultiple;
        DetectMonster();
    }

    public override void DoPassive(float time)
    {
        if (!isScratching)
            return;

        hitTimer += time;
        if (hitTimer < data.hitInterval)
            return;

        Scratch(targetMonster);
        hitTimer = DefaultZero;

        hitCount++;
        if (hitCount >= data.hitCount)
        {
            isScratching = false;
        }
    }

    void DetectMonster()
    {
        Collider2D[] enemyCols = Physics2D.OverlapBoxAll(Player.transform.position, detectSize, DefaultZero, data.enemyLayer);
        targetMonster = GetMonster(enemyCols);
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

    void Scratch(Monster monster)
    {
        if (monster == null || !monster.gameObject.activeInHierarchy)
            return;

        ScratchEffect(monster.transform);
        monster.TakeDamage(damage);
    }

    void ScratchEffect(Transform target)
    {
        if (target == null)
            return;

        EffectPoolManager manager = Player.EffectPoolManager;
        Effect effect = manager.GetObj();
        effect.transform.position = target.position;
        effect.InitData(manager, data.effect, Vector2.zero, data.effectSize, target);
    }
}
