using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeFlower : RangerMonster
{
    [SerializeField] private float idleTime = 5.0f;
    [SerializeField] private float moveTime = 2.0f;
    [SerializeField] private ToxinCloud toxinCloudEffect;

    public float IdleTime => idleTime;
    public float MoveTime => moveTime;

    private SporeFlowerIdleState _idleState;
    private SporeFlowerMoveState _moveState;
    private SporeFlowerAttackState _attackState;
    private SporeFlowerDieState _dieState;

    private bool _hasMoved = false;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new SporeFlowerIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new SporeFlowerMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new SporeFlowerAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new SporeFlowerDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();

        if (toxinCloudEffect == null)
        {
            toxinCloudEffect = GetComponentInChildren<ToxinCloud>(true);
        }

        if (toxinCloudEffect != null)
        {
            toxinCloudEffect.gameObject.SetActive(false);
            toxinCloudEffect.transform.localPosition = Vector3.zero;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        CheckHp();
    }

    public void OnAttack()
    {
        if(toxinCloudEffect != null)
        {
            toxinCloudEffect.StartToxinCloudEffect(Damage);
        }
    }

    private void CheckHp()
    {
        if (toxinCloudEffect != null && toxinCloudEffect.gameObject.activeInHierarchy)
        {
            toxinCloudEffect.EndToxinCloudEffect();
        }

        float halfPercentHp = MaxHp * 0.5f;

        if (Hp <=  halfPercentHp && !_hasMoved)
        {
            if (MonsterStateManager.CurState != CreateDieState())
            {
                _hasMoved = true;
                MonsterStateManager.ChangeState(CreateMoveState());
            }
        }
    }
}
