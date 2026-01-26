using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class LightningSkill : IActiveSKill
{
    LightningData data;
    Collider2D[] enemyCols;
    float timer;
    int count;

    float totalDamage;
    Vector2 detectSize;
    public Player Player { get; private set; } 

    public bool IsFinish { get; private set; }

    public LightningSkill(PlayerSkill playerSkill, LightningData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }
    public void Enter()
    {
        ResetValue();
        detectSize = new Vector2(data.boxSize, data.boxSize);
        totalDamage = Player.PlayerAttack.Damage * Player.PlayerAttack.DamageMultiple * data.damageMultiple;
        DetectTarget();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (enemyCols.Length == DefaultIntZero)
        {
            IsFinish = true;
            return;
        }
        timer += Time.deltaTime;
        if(timer >= data.interval && count < data.count)
        {
            Monster monster = GetMonster();
            if(monster != null)
            {
                Lightning(monster.transform);
                monster.TakeDamage(totalDamage);
            }
            timer = DefaultZero;
            count++;
            if (count == data.count)
            {
                IsFinish = true;
            }
        }
    }

    void ResetValue()
    {
        IsFinish = false;
        timer = DefaultZero;
        count = DefaultIntZero;
    }

    void DetectTarget()
    {
         enemyCols = Physics2D.OverlapBoxAll(Player.transform.position, detectSize, DefaultZero, data.enemyLayer);
    }

    Monster GetMonster()
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
        EffectPoolManager manager = Player.EffectPoolManager;
        Effect effect = manager.GetObj();
        effect.transform.position = target.position;
        effect.InitData(manager, data.lightningEffect, Vector2.zero, data.size, target);
    }
}
