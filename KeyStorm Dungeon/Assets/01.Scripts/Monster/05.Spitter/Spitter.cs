using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : RangerMonster
{
    private SpitterIdleState _idleState;
    private SpitterMoveState _moveState;    // 움직이지 않기에 사용하지않음
    private SpitterAttackState _attackState;
    private SpitterDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        return base.CreateIdleState();
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        return base.CreateMoveState();
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        return base.CreateAttackState();
    }

    public override CharacterState<Monster> CreateDieState()
    {
        return base.CreateDieState();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void OnAttack(Character character)
    {
        
    }
}
