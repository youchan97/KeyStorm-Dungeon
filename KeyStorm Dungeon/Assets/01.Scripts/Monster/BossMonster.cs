using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Monster
{
    [Header("보스 패턴 수치 설정")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float diveDelay;
    [SerializeField] private float diveDuration;
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float shadowScaleTime;
    [SerializeField] private float minShadowScale = 0.1f;
    [SerializeField] private float maxShadowScale = 2.0f;

    public float JumpHeight => jumpHeight;
    public float JumpDuration => jumpDuration;
    public float DiveDelay => diveDelay;
    public float DiveDuration => diveDuration;
    public GameObject ShadowPrefab => shadowPrefab;
    public float ShadowScaleTime => shadowScaleTime;
    public float MinShadowScale => minShadowScale;
    public float MaxShadowScale => maxShadowScale;

    private MonsterIdleState _idleState;
    private BossMonsterMoveState _moveState;
    private BossMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_idleState == null) _idleState = new MonsterIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_moveState == null) _moveState = new BossMonsterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_attackState == null) _attackState = new BossMonsterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_dieState == null) _dieState = new MonsterDieState(this, MonsterStateManager);
        return _dieState;
    }
}
