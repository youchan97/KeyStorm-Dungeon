using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenButterfly : MeleeMonster
{
    [SerializeField] private float idleMoveTime;
    [SerializeField] private float idleRange;
    [SerializeField] private float idleMoveSpeedMultiplier;

    private Vector3 spawnPosition;

    public float IdleMoveTime => idleMoveTime;
    public float IdleRange => idleRange;
    public float IdleMoveSpeedMultiplier => idleMoveSpeedMultiplier;
    
    public Vector3 SpawnPosition => spawnPosition;



    private PollenButterflyIdleState _idleState;
    private PollenButterflyMoveState _moveState;
    private PollenButterflyAttackState _attackState;
    private PollenButterflyDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new PollenButterflyIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new PollenButterflyMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new PollenButterflyAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new PollenButterflyDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
        spawnPosition = transform.position;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    // 꽃가루 소환 메서드
}
