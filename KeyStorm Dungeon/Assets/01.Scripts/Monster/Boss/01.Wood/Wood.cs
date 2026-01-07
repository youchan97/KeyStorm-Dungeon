using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MeleeMonster
{
    [Header("상태 전환 타이머")]
    [SerializeField] private float idleTime; // Idle상태에 머무르는 시간
    [SerializeField] private float moveDelay; // Move 다음 Move까지의 딜레이

    [Header("공통 패턴 수치")]
    [SerializeField] private float attackDelay; // 공격 이후 후딜레이 -> Idle상태에 머무를 시간
    [SerializeField] private int minFootStepCount; // 최소 몇 발자국 마다 Attack상태로 넘어갈 것인지
    [SerializeField] private int maxFootStepCount; // 최대 발자국
    [SerializeField] private LayerMask damageLayers; // 패턴에 영향을 받는 레이어 (플레이어, 뿌리)
    [SerializeField] private LayerMask rootLayer;   // 뿌리 레이어

    public float IdleTime => idleTime;
    public float MoveDelay => moveDelay;
    public float AttackDelay => attackDelay;
    public int MinFootStepCount => minFootStepCount;
    public int MaxFootStepCount => maxFootStepCount;
    

    [Header("돌진 패턴 수치")]
    private float rushMoveSpeed;

    [Header("도약 패턴 수치")]
    [SerializeField] private float jumpHeight;  // 점프 높이
    [SerializeField] private float jumpDuration;// 최대 높이까지 걸리는 시간
    [SerializeField] private float diveDelay;   // 체공 시간
    [SerializeField] private float diveDuration;// 착지까지 걸리는 시간
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float shadowScaleTime; // 그림자 크기 변경 시간
    [SerializeField] private float minShadowScale = 0.1f;
    [SerializeField] private float maxShadowScale = 1.0f;
    [SerializeField] private float landedDelay; // 착지 이후 후딜레이

    [Header("뿌리 패턴 수치")]
    [SerializeField] private int spawnRootQuantity;     // 뿌리 소환 개수
    [SerializeField] private float spawnRootDuration;   // 뿌리 소환간의 간격
    [SerializeField] private GameObject woodsRoot;    // 뿌리 자체의 게임 오브젝트
    [SerializeField] private float rootPatternCooldown; // 패턴 자체 쿨타임

    public float CurrentRootPatternCooldown { get; private set; }

    public Action OnTakeOneStepAnimation;

    private WoodIdleState _idleState;
    private WoodMoveState _moveState;
    private WoodAttackState _attackState;
    private WoodDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new WoodIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new WoodMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new WoodAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new WoodDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if(CurrentRootPatternCooldown > 0f)
        {
            CurrentRootPatternCooldown -= Time.deltaTime;
        }
    }

    public void ApplyLandingDamage(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius, damageLayers);
        foreach (var hitCollider in hitColliders)
        {
            if (((1 << hitCollider.gameObject.layer) & playerLayer.value) > 0)
            {
                Player hitPlayer = hitCollider.GetComponent<Player>();
                if (hitPlayer != null)
                {
                    hitPlayer.TakeDamage(Damage);
                }
            }
            else if(((1 << hitCollider.gameObject.layer) & rootLayer.value) > 0)
            {
                WoodsRoot hitRoot = hitCollider.GetComponent<WoodsRoot>();
                if (hitRoot != null)
                {
                    hitRoot.Die();
                }
            }
        }
    }

    public void OnTakeOneStep()
    {
        OnTakeOneStepAnimation?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    public void ResetRootPatternCooldown()
    {
        CurrentRootPatternCooldown = rootPatternCooldown;
    }
}
