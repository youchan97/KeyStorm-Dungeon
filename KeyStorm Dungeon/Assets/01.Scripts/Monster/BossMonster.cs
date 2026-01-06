using System;
using UnityEngine;
using System.Collections;

public class BossMonster : Monster
{
    [Header("보스 패턴 수치 설정")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float diveDelay;
    [SerializeField] private float diveDuration;
    [SerializeField] private GameObject bossShadowPrefab;
    [SerializeField] private float shadowScaleTime;
    [SerializeField] private float minShadowScale = 0.1f;
    [SerializeField] private float maxShadowScale = 1.0f;
    [SerializeField] private float landedDelay;
    [SerializeField] private float patternCooldown;

    public float JumpHeight => jumpHeight;
    public float JumpDuration => jumpDuration;
    public float DiveDelay => diveDelay;
    public float DiveDuration => diveDuration;
    public GameObject BossShadowPrefab => bossShadowPrefab;
    public float ShadowScaleTime => shadowScaleTime;
    public float MinShadowScale => minShadowScale;
    public float MaxShadowScale => maxShadowScale;
    public float LandedDelay => landedDelay;
    public float PatternCooldown => patternCooldown;

    public float CurrentPatternCooldown { get; private set; }


    private BossIdleState _idleState;
    private BossMonsterMoveState _moveState;
    private BossMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public Action OnJumpAnimationFinished;

    protected override void Start()
    {
        base.Start();
    }

    public void OnJumpAnimationEnd()
    {
        OnJumpAnimationFinished?.Invoke();
    }

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new BossIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new BossMonsterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new BossMonsterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MonsterDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Update()
    {
        base.Update();
        if(CurrentPatternCooldown > 0f)
        {
            CurrentPatternCooldown -= Time.deltaTime;
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
                Debug.Log($"낙하 공격으로 {hitPlayer}에게 {Damage}");
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if (CurrentAttackCooldown <= 0f)
        {
            if (currentGameObject.CompareTag("Player"))
            {
                Player player = currentGameObject.GetComponent<Player>();

                if (player != null)
                {
                    Attack(player);
                    Debug.Log("공격!");
                    ResetAttackCooldown();
                }
            }

        }
    }

    public void ResetPatternCooldown()
    {
        CurrentPatternCooldown = patternCooldown;
    }
}
