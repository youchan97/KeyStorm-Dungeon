using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    [SerializeField] private MonsterData monsterData;

    #region Property
    public int DetectRange {  get; private set; }
    public int Tier { get; private set; }
    public float TargetDistance { get; private set; }
    public float ShotSpeed { get; private set; }
    public float AttackSpeed { get; private set; }
    public int DropGold { get; private set; }
    public int MinStage { get; private set; }
    public int MaxStage { get; private set; }
    public float XScale { get; private set; }
    public float YScale { get; private set; }
    #endregion

    private void InitMonsterData(MonsterData monsterData)
    {
        DetectRange = (int)monsterData.detectRange;
        Tier = monsterData.tier;
        TargetDistance = monsterData.targetDistance;
        ShotSpeed = monsterData.shotSpeed;
        AttackSpeed = monsterData.attackSpeed;
        DropGold = monsterData.dropGold;
        MinStage = monsterData.minStage;
        MaxStage = monsterData.maxStage;
        XScale = monsterData.xScale;
        YScale = monsterData.yScale;
    }

    public override void Attack(Character character)
    {
        base.Attack(character);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
    }
}
