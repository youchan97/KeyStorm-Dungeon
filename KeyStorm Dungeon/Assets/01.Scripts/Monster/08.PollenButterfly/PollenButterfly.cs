using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenButterfly : MeleeMonster
{
    [Header("꽃가루나비 설정")]
    [SerializeField] private float idleMoveTime;
    [SerializeField] private float idleRange;
    [SerializeField] private float idleMoveSpeedMultiplier;
    [SerializeField] private GameObject pollenBurstEffectPrefab;

    private Vector3 spawnPosition;

    public float IdleMoveTime => idleMoveTime;
    public float IdleRange => idleRange;
    public float IdleMoveSpeedMultiplier => idleMoveSpeedMultiplier;
    
    public Vector3 SpawnPosition => spawnPosition;

    private const string PollenBurstEffectPool = "PollenBurstEffect";

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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (MonsterStateManager.CurState != _attackState && MonsterStateManager.CurState != _dieState)
        {
            MonsterStateManager.ChangeState(CreateAttackState());
        }
    }

    public override void Die()
    {
        pollenEffect();
        base.Die();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    private void pollenEffect()
    {
        if (pollenBurstEffectPrefab != null)
        {
            GameObject burstEffect = ObjectPoolManager.Instance.GetObject(PollenBurstEffectPool, transform.position, Quaternion.identity);

            if(burstEffect != null)
            {
                burstEffect.SetActive(true);
            }
        }
    }
}
