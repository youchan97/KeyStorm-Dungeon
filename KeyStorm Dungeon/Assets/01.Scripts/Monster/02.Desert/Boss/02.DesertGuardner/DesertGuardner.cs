using System;
using UnityEngine;
using static ConstValue;

public class DesertGuardner : MeleeMonster
{
    [Header("상태 수치")]
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;

    [Header("회전 패턴 수치")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private float spinPatternDuration; // 패턴 지속 시간
    [SerializeField] private float spinRotationSpeed;   // 스프라이트 회전 시간(탄 발사용)
    [SerializeField] private float spinRadius;          // 스프라이트 회전 반지름(탄 발사용)
    [SerializeField] private Sprite bulletSprite;       // 탄막 이미지
    [SerializeField] private float spinPatternMoveSpeed;
    [SerializeField] private float bulletInterval;      // 탄막 간 생성 대기 시간


    [Header("휘두르기 패턴 수치")]
    [SerializeField] private float swingDelay;
    [SerializeField] private float swingAngle;
    [SerializeField] private int firstSwingBulletCount;
    [SerializeField] private int secondSwingBulletCount;

    [Header("반사 패턴 수치")]
    [SerializeField] private float reflectPatternCooldown; // 반사 패턴 자체 쿨타임
    [SerializeField] private float reflectPatternDuration; // 반사 패턴 지속시간


    private bool isSpin;    // 회전 패턴 중임을 알림
    private bool isReflect; // 반사 패턴 중임을 알림

    #region 속성
    public SpriteRenderer SpriteRenderer => monsterSpriteRenderer;
    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float SpinPatternDuration => spinPatternDuration;
    public float SpinRotationSpeed => spinRotationSpeed;
    public float SpinRadius => spinRadius;
    public float SpinPatternMoveSpeed => spinPatternMoveSpeed;
    public float BulletInterval => bulletInterval;
    public float SwingDelay => swingDelay;
    public float ReflectPatternCooldown => reflectPatternCooldown;
    public float ReflectPatternDuration => reflectPatternDuration;
    public bool IsSpin => isSpin;
    public bool IsReflect => isReflect;
    public float CurrentReflectPatternCooldown {  get; private set; }
    #endregion

    #region 애니메이션
    private const string ReflectAnim = "Reflect";
    #endregion

    public event Action OnTakeShieldAnimation;
    public event Action OnTakeOffShieldAnimation;

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

        CurrentReflectPatternCooldown = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if (CurrentReflectPatternCooldown > 0f)
        {
            CurrentReflectPatternCooldown -= Time.deltaTime;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (isReflect)
        {
            audioManager.PlayEffect(DesertGuardnerReflectionSfx);
            Animator.SetTrigger(ReflectAnim);
            ReflectBullet();
            return;
        }

        base.TakeDamage(damage);
    }

    private void ReflectBullet()
    {
        Vector2 playerDirection = (PlayerTransform.position - transform.position).normalized;
        FireBullet(playerDirection);
    }

    public void FireBullet(Vector2 direction)
    {
        AttackObj pooledAttackObject = attackPoolManager.GetObj();

        pooledAttackObject.transform.position = shootPoint.position;
        pooledAttackObject.transform.rotation = Quaternion.identity;
        pooledAttackObject.InitData(bulletSprite, Damage, direction, MonsterData.shotSpeed, bulletLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius, null);
    }

    public void StartSpin()
    {
        isSpin = true;
    }

    public void StopSpin()
    {
        isSpin = false;
        MonsterRb.velocity = Vector2.zero;
    }

    public void StartReflect()
    {
        isReflect = true;
    }

    public void StopReflect()
    {
        isReflect = false;
    }

    private void FireBulletFromSwingAttack(int bulletCount)
    {
        if (attackPoolManager == null) return;

        audioManager.PlayEffect(DesertGuardnerSwingSfx);

        Vector2 playerDirection = (PlayerTransform.position - transform.position).normalized;
        float halfAngle = swingAngle * 0.5f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletAngle;
            if (bulletCount <= 1)
            {
                bulletAngle = 0.5f;
            }
            else
            {
                bulletAngle = (float)i / (bulletCount - 1);
            }

            float angle = Mathf.Lerp(-halfAngle, halfAngle, bulletAngle);

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

    public void OnFirstSwingAttack()
    {
        FireBulletFromSwingAttack(firstSwingBulletCount);
    }

    public void OnSecondSwingAttack()
    {
        FireBulletFromSwingAttack(secondSwingBulletCount);
    }

    public override void FlipSprite(float moveDirectionX)
    {
        if (monsterSpriteRenderer == null) return;

        if (moveDirectionX < 0)
        {
            monsterSpriteRenderer.flipX = true;
        }
        else if (moveDirectionX > 0)
        {
            monsterSpriteRenderer.flipX = false;
        }
    }

    public void ResetFilpSprite()
    {
        monsterSpriteRenderer.flipX = false;
    }

    public void OnTakeShield()
    {
        OnTakeShieldAnimation?.Invoke();
    }

    public void OnTakeOffShield()
    {
        OnTakeOffShieldAnimation?.Invoke();
    }

    public void ResetReflectPatternCooldown()
    {
        CurrentReflectPatternCooldown = reflectPatternCooldown;
    }
}
