using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertGuardner : MeleeMonster
{
    [Header("상태 수치")]
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;

    [Header("회전 패턴 수치")]
    [SerializeField] private float spinPatternDuration; // 패턴 지속 시간
    [SerializeField] private float spinRotationSpeed;   // 스프라이트 회전 시간(탄 발사용)
    [SerializeField] private float spinRadius;          // 스프라이트 회전 반지름(탄 발사용)
    [SerializeField] private Sprite bulletSprite;       // 탄막 이미지
    
    /*[Header("휘두르기 패턴 수치")]

    [Header("방어 패턴 수치")]*/

    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float SpinPatternDuration => spinPatternDuration;
    public float SpinRotationSpeed => spinRotationSpeed;
    public float SpinRadius => spinRadius;

    private DesertGuardnerIdleState _idleState;
    private DesertGuardnerMoveState _moveState;
    private DesertGuardnerAttackState _attackState;
    private DesertGuardnerDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new DesertGuardnerIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new DesertGuardnerMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new DesertGuardnerAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new DesertGuardnerDieState(this, MonsterStateManager);
        return _dieState;
    }
}
