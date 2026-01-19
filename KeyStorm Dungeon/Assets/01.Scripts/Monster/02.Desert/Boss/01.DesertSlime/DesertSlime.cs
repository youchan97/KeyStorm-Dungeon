using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSlime : Slime
{
    [SerializeField] private float slideSpeed;

    public float SlideSpeed => slideSpeed;
    public bool IsSlide { get; private set; }

    private DesertSlimeIdleState _idleState;
    private DesertSlimeMoveState _moveState;
    private DesertSlimeAttackState _attackState;
    private DesertSlimeDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new DesertSlimeIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new DesertSlimeMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new DesertSlimeAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new DesertSlimeDieState(this, MonsterStateManager);
        return _dieState;
    }


    public void StartSlide()
    {
        IsSlide = true;
    }

    public void StopSlide()
    {
        IsSlide = false;
        MonsterRb.velocity = Vector2.zero;
    }
}
