using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatButterfly : MeleeMonster
{
    // 플레이어에게 이동 하는 몬스터, 패턴 없음

    private CombatButterflyIdleState _idleState;
    private CombatButterflyMoveState _moveState;
    private CombatButterflyAttackState _attackState;
    private CombatButterflyDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new CombatButterflyIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new CombatButterflyMoveState(this, MonsterStateManager);
        return _moveState;
    }

    // 공격 패턴이 없어 실질적으로 사용하지않음
    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new CombatButterflyAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new CombatButterflyDieState(this, MonsterStateManager);
        return _dieState;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(MonsterStateManager.CurState != _attackState && MonsterStateManager.CurState != _dieState)
        {
            MonsterStateManager.ChangeState(CreateAttackState());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }
}
