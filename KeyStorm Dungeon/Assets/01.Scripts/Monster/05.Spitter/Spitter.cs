using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : RangerMonster
{
    [SerializeField] private float projectileLifeTime;

    private SpitterIdleState _idleState;
    private SpitterMoveState _moveState;    // 움직이지 않기에 사용하지않음
    private SpitterAttackState _attackState;
    private SpitterDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new SpitterIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new SpitterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new SpitterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new SpitterDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void OnAttack()
    {
        if (attackPoolManager == null) return;

        if (shootPoint == null)
        {
            shootPoint = this.transform;
        }

        AttackObj pooledAttackObj = attackPoolManager.GetAttack();
        if (pooledAttackObj == null)
        {
            return;
        }

        Vector2 porjectileDirection = (player.transform.position - shootPoint.position).normalized;

        pooledAttackObj.transform.position = shootPoint.position;
        pooledAttackObj.transform.rotation = Quaternion.identity;

        pooledAttackObj.InitData(bullet, Damage, porjectileDirection, MonsterData.shotSpeed, projectileLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius);
    }
}
