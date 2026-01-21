using System;
using UnityEngine;

public class DesertSlime : Slime
{
    [Header("돌진 패턴 수치")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideStopDistance;

    public float SlideSpeed => slideSpeed;
    public float SlideStopDistance => slideStopDistance;
    public bool IsSlide { get; private set; }

    private DesertSlimeIdleState _idleState;
    private DesertSlimeMoveState _moveState;
    private DesertSlimeAttackState _attackState;
    private DesertSlimeDieState _dieState;

    public event Action OnReadySlideAnimation;
    public event Action OnSpinAnimation;

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

    public void OnReadySlide()
    {
        OnReadySlideAnimation?.Invoke();
    }

    public void OnSpin()
    {
        OnSpinAnimation?.Invoke();
    }
}
