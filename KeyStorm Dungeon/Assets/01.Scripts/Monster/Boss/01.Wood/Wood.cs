using System;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MeleeMonster
{
    [Header("상태 전환 타이머")]
    [SerializeField] private float idleTime; // Idle상태에 머무르는 시간
    [SerializeField] private float moveDelay; // Move 다음 Move까지의 딜레이

    [Header("공통 패턴 수치")]
    [SerializeField] private float attackDelay; // 공격 이후 후딜레이 -> Idle상태로 가기전 머무를 시간
    [SerializeField] private int minFootStepCount; // 최소 몇 발자국 마다 Attack상태로 넘어갈 것인지
    [SerializeField] private int maxFootStepCount; // 최대 발자국
    [SerializeField] private LayerMask damageLayers; // 패턴에 영향을 받는 레이어 (플레이어, 뿌리)
    [SerializeField] private LayerMask rootLayer;   // 뿌리 레이어

    [Header("돌진 패턴 수치")]
    [SerializeField] private float dashMoveSpeed;
    [SerializeField] private LayerMask hitToDashStopLayer;
    [SerializeField] private float detectStopDistance;
    [SerializeField] private Collider2D woodCollider;

    [Header("도약 패턴 수치")]
    [SerializeField] private float jumpHeight;  // 점프 높이
    [SerializeField] private float jumpDuration;// 최대 높이까지 걸리는 시간
    [SerializeField] private float diveDelay;   // 체공 시간
    [SerializeField] private float diveDuration;// 착지까지 걸리는 시간
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float minShadowScale;
    [SerializeField] private float maxShadowScale;
    [SerializeField] private float shadowOffsetYDown;
    [SerializeField] private float diveAttackRange;
    [SerializeField] private float diveYOffset;

    [Header("뿌리 패턴 수치")]
    [SerializeField] private int spawnRootQuantity;     // 뿌리 소환 개수
    [SerializeField] private float spawnRootDuration;   // 뿌리 소환간의 간격
    [SerializeField] private GameObject woodsRoot;    // 뿌리 자체의 게임 오브젝트
    [SerializeField] private float rootPatternCooldown; // 패턴 자체 쿨타임
    [SerializeField] private float playerSearchTime;

    private List<WoodsRoot> woodsroots = new List<WoodsRoot>();

    #region 속성
    public float IdleTime => idleTime;
    public float MoveDelay => moveDelay;
    public float AttackDelay => attackDelay;
    public int MinFootStepCount => minFootStepCount;
    public int MaxFootStepCount => maxFootStepCount;
    public float DashMoveSpeed => dashMoveSpeed;
    public LayerMask HitToDashStopLayer => hitToDashStopLayer;
    public float DetectStopDistance => detectStopDistance;
    public Collider2D WoodCollider => woodCollider;
    public float JumpHeight => jumpHeight;
    public float JumpDuration => jumpDuration;
    public float DiveDelay => diveDelay;
    public float DiveDuration => diveDuration;
    public GameObject BossShadowPrefab => bossShadowPrefab;
    public float MinShadowScale => minShadowScale;
    public float MaxShadowScale => maxShadowScale;
    public float ShadowOffset => shadowOffsetYDown;
    public float DiveAttackRange => diveAttackRange;
    public float DiveYOffset => diveYOffset;
    public int SpawnRootQuantity => spawnRootQuantity;
    public float SpawnRootDuration => spawnRootDuration;
    public float PlayerSearchTime => playerSearchTime;
    public bool IsDash { get; private set; }
    public float CurrentRootPatternCooldown { get; private set; }
    #endregion

    public event Action OnTakeOneStepAnimation;
    public event Action OnReadyToDashAnimation;
    public event Action OnJumpAnimation;
    public event Action OnTakeRootAnimation;

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

        if (CurrentRootPatternCooldown > 0f)
        {
            CurrentRootPatternCooldown -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Die()
    {
        base.Die();

        foreach (WoodsRoot root in woodsroots)
        {
            if (root != null)
            {
                root.Die();
            }
        }
        woodsroots.Clear();
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
            else if (((1 << hitCollider.gameObject.layer) & rootLayer.value) > 0)
            {
                WoodsRoot hitRoot = hitCollider.GetComponent<WoodsRoot>();
                if (hitRoot != null)
                {
                    hitRoot.Die();
                }
            }
        }
    }

    #region 애니메이션이 끝남을 알리는 메서드
    // 한걸음 걸었음을 알림 애니메이터에서 호출
    public void OnTakeOneStep()
    {
        OnTakeOneStepAnimation?.Invoke();
    }

    // 돌진 준비자세가 끝났음을 알림 애니메이터에서 호출
    public void OnReadyToDash()
    {
        OnReadyToDashAnimation?.Invoke();
    }

    // 점프 준비 애니메이션이 끝났음을 알림
    public void OnJump()
    {
        OnJumpAnimation?.Invoke();
    }

    // 뿌리 박기 애니메이션이 끝났음을 알림
    public void OnTakeRoot()
    {
        OnTakeRootAnimation?.Invoke();
    }
    #endregion

    private void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsDash)
        {
            if (((1 << collision.gameObject.layer) & hitToDashStopLayer.value) > 0)
            {
                if (((1 << collision.gameObject.layer) & rootLayer.value) > 0)
                {
                    WoodsRoot hitRoot = collision.GetComponent<WoodsRoot>();
                    if (hitRoot != null)
                    {
                        hitRoot.Die();
                        StopDash();
                    }
                }
            }
        }
    }

    public void StartDash()
    {
        IsDash = true;
    }

    public void StopDash()
    {
        IsDash = false;
        MonsterRb.velocity = Vector2.zero;
    }

    public void ResetRootPatternCooldown()
    {
        CurrentRootPatternCooldown = rootPatternCooldown;
    }

    public WoodsRoot SpawnWoodsRoot(Vector3 position)
    {
        if (woodsRoot == null) return null;

        GameObject rootGO = Instantiate(woodsRoot, position, Quaternion.identity);

        WoodsRoot newWoodsRoot = rootGO.GetComponent<WoodsRoot>();

        if (MyRoom != null)
        {
            newWoodsRoot.SetMyRoom(MyRoom);
            MyRoom.AddMonster(newWoodsRoot);
        }

        woodsroots.Add(newWoodsRoot);

        return newWoodsRoot;
    }
}
