using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MeleeMonster
{
    [SerializeField] private float idleTime; // Idle상태에 머무르는 시간

    [Header("탄막 정보")]
    [SerializeField] private Transform shootPoint;  // 탄막 생성 위치 내려찍기패턴과 도약패턴일 때의 위치가 다를듯
    [SerializeField] private Sprite bullet;         // 탄막 이미지

    [Header("도약 패턴 수치")]
    [SerializeField] private float jumpHeight;  // 점프 높이
    [SerializeField] private float jumpDuration;// 최대 높이까지 걸리는 시간
    [SerializeField] private float diveDelay;   // 체공 시간
    [SerializeField] private float diveDuration;// 착지까지 걸리는 시간
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float shadowScaleTime; // 그림자 크기 변경 시간
    [SerializeField] private float minShadowScale;
    [SerializeField] private float maxShadowScale;
    [SerializeField] private float shadowOffset;

    private SlimeIdleState _idleState;
    private SlimeMoveState _moveState;
    private SlimeAttackState _attackState;
    private SlimeDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new SlimeIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new SlimeMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new SlimeAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new SlimeDieState(this, MonsterStateManager);
        return _dieState;
    }
}
