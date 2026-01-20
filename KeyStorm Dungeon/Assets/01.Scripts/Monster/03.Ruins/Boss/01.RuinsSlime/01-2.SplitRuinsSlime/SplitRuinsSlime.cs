using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitRuinsSlime : RuinsSlime
{
    [Header("2차 분열 사원 슬라임")]
    [SerializeField] private TwiceSplitRuinsSlime twiceSplitRuinsSlime;

    private RuinsSlimeIdleState _idleState;
    private RuinsSlimeMoveState _moveState;
    private RuinsSlimeAttackState _attackState;
    private RuinsSlimeDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new RuinsSlimeIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new RuinsSlimeMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new RuinsSlimeAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new RuinsSlimeDieState(this, MonsterStateManager);
        return _dieState;
    }

    public override void Die()
    {
        base.Die();

    }
}
