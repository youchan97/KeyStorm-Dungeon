using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needlehog : RangerMonster
{
    private float idleTime = 1.0f;
    private float moveTime = 3.0f;

    public float IdleTime => idleTime;
    public float MoveTime => moveTime;

    private NeedlehogIdleState _idleState;
    private NeedlehogMoveState _moveState;
    private NeedlehogAttackState _attackState;
    private NeedlehogDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new NeedlehogIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new NeedlehogMoveState(this, MonsterStateManager);
        return _moveState;
    }

}
