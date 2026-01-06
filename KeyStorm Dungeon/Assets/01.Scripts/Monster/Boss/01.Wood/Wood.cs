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
    [SerializeField] private LayerMask rootLayer;

    public float IdleTime => idleTime;
    public float MoveDelay => moveDelay;
    public float AttackDelay => attackDelay;
    public int MinFootStepCount => minFootStepCount;
    public int MaxFootStepCount => maxFootStepCount;
    


    [Header("돌진 패턴 수치")]
    private float moveSpeed;

    [Header("도약 패턴 수치")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float diveDelay;
    [SerializeField] private float diveDuration;
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float shadowScaleTime;
    [SerializeField] private float minShadowScale = 0.1f;
    [SerializeField] private float maxShadowScale = 1.0f;

    [Header("뿌리 패턴 수치")]
    [SerializeField] private int spawnRootQuantity;     // 뿌리 소환 개수
    [SerializeField] private float spawnRootDuration;   // 뿌리 소환간의 간격
    [SerializeField] private GameObject woodsRoot;    // 뿌리 자체의 게임 오브젝트
    [SerializeField] private float rootPatternCooldown; // 패턴 자체 쿨타임

    public float CurrentRootPatternCooldown { get; private set; }

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
            if (playerLayer == (playerLayer | (1 << hitCollider.gameObject.layer)))
            {
                Player hitPlayer = hitCollider.GetComponent<Player>();
                if (hitPlayer != null)
                {
                    hitPlayer.TakeDamage(Damage);
                }
            }
            else if(rootLayer == (rootLayer | (1 << hitCollider.gameObject.layer)))
            {
                WoodsRoot hitRoot = hitCollider.GetComponent<WoodsRoot>();
                if (hitRoot != null)
                {
                    //hitRoot.MonsterStateManager.ChangeState(ChangeDieState());
                }
            }
        }
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
