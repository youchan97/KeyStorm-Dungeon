using System;
using UnityEngine;
using static ConstValue;

public class Slime : MeleeMonster
{
    [SerializeField] private float idleTime; // Idle상태에 머무르는 시간

    [Header("이동 패턴 수치")]
    [SerializeField] private float moveDelay;
    [SerializeField] private int moveNumber;    // 움직일 횟수

    [Header("내려찍기 패턴 수치")]
    [SerializeField] private Transform shootPoint;    // 탄막 생성 위치 내려찍기패턴과 도약패턴일 때의 위치가 다를듯
    [SerializeField] private Sprite bulletSprite;       // 탄막 이미지
    [SerializeField] private float slamDelay;         // 패턴 수행 이후 후딜레이
    [SerializeField] private float slamAngle;         // 탄막 발사 각도
    [SerializeField] private int bulletCount;     // 탄막 갯수
    [SerializeField] private float bulletLifeTime;// 탄막 생존 시간

    [Header("도약 패턴 수치")]
    [SerializeField] private float jumpHeight;        // 점프 높이
    [SerializeField] private float jumpDuration;      // 최대 높이까지 걸리는 시간
    [SerializeField] private float diveDelay;         // 체공 시간
    [SerializeField] private float diveDuration;      // 착지까지 걸리는 시간
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float shadowScaleTime;   // 그림자 크기 변경 시간
    [SerializeField] private float minShadowScale;
    [SerializeField] private float maxShadowScale;
    [SerializeField] private float shadowOffset;
    [SerializeField] private float maxShadowDuration; // 착지 전 그림자 최대 크기 노출 시간
    [SerializeField] private float diveAttackRange;   // 착지 공격 범위
    [SerializeField] private float diveAttackDelay;

    #region 8방향 벡터
    private Vector2[] eightDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
    };
    #endregion

    #region 속성
    public float IdleTime => idleTime;
    public float MoveDelay => moveDelay;
    public int MoveNumber => moveNumber;
    public float SlamDelay => slamDelay;
    public float JumpHeight => jumpHeight;
    public float JumpDuration => jumpDuration;
    public float DiveDelay => diveDelay;
    public float DiveDuration => diveDuration;
    public GameObject BossShadowPrefab => bossShadowPrefab;
    public float ShadowScaleTime => shadowScaleTime;
    public float MinShadowScale => minShadowScale;
    public float MaxShadowScale => maxShadowScale;
    public float ShadowOffset => shadowOffset;
    public float MaxShadowDuration => maxShadowDuration;
    public float DiveAttackRange => diveAttackRange;
    public float DiveAttackDelay => diveAttackDelay;
    public bool IsMove { get; private set; }
    #endregion

    public event Action OnJumpMoveReadyAnimation;
    public event Action OnJumpMoveLandAnimation;
    public event Action OnJumpAnimation;

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

    protected override void Awake()
    {
        base.Awake();
        if (shootPoint == null)
        {
            shootPoint = this.transform;
        }

        if (attackPoolManager == null)
        {
            attackPoolManager = FindObjectOfType<AttackPoolManager>();
        }
    }

    public void OnJumpMoveReady()
    {
        OnJumpMoveReadyAnimation?.Invoke();
    }

    public void OnJumpMoveLand()
    {
        OnJumpMoveLandAnimation?.Invoke();
    }

    public void OnJump()
    {
        OnJumpAnimation?.Invoke();
    }

    public void StartMove()
    {
        IsMove = true;
    }

    public void StopMove()
    {
        IsMove = false;
        MonsterRb.velocity = Vector2.zero;
    }

    public override void FlipSpriteAttack(Transform playerTransform)
    {
        if (playerTransform == null && monsterSpriteRenderer == null) return;

        if (playerTransform.position.x < MonsterRb.position.x)
        {
            monsterSpriteRenderer.flipX = true;
        }
        else
        {
            monsterSpriteRenderer.flipX = false;
        }
    }

    public void OnSlamAttack()
    {
        audioManager.PlayEffect(SlimeSlamSfx);

        Vector2 playerDirection = (PlayerTransform.position - transform.position).normalized;
        if (attackPoolManager == null) return;

        float halfAngle = slamAngle * 0.5f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletAnglePart = (float)i / (bulletCount - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, bulletAnglePart);

            Vector3 rotateDirection = Quaternion.Euler(0, 0, angle) * (Vector3)playerDirection;
            Vector2 bulletDirection = rotateDirection;

            AttackObj pooledAttackObject = attackPoolManager.GetObj();
            
            if (pooledAttackObject == null) return;
            if (player == null) return;

            pooledAttackObject.transform.position = shootPoint.position;
            pooledAttackObject.transform.rotation = Quaternion.identity;

            pooledAttackObject.InitData(bulletSprite, Damage, bulletDirection, MonsterData.shotSpeed, bulletLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius, null);
        }
    }

    public void OnDiveAttack()
    {
        if (attackPoolManager == null) return;

        foreach(var direction in eightDirections)
        {
            AttackObj pooledAttackObject = AttackPoolManager.GetObj();

            pooledAttackObject.transform.position = shootPoint.position;
            pooledAttackObject.transform.rotation = Quaternion.identity;
            pooledAttackObject.InitData(bulletSprite, Damage, direction, MonsterData.shotSpeed, bulletLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius, null);
        }
    }

    public void ApplyLandingDamage(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(Damage);
            }
        }
    }
}
