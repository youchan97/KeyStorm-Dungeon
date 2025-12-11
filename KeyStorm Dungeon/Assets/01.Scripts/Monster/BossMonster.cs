using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Monster
{
    private MonsterIdleState _idleState;
    private BossMonsterMoveState _moveState;
    private BossMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public override CharacterState<Monster> CreateAttackState()
    {
        throw new System.NotImplementedException();
    }

    public override CharacterState<Monster> CreateDieState()
    {
        throw new System.NotImplementedException();
    }

    public override CharacterState<Monster> CreateIdleState()
    {
        throw new System.NotImplementedException();
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        throw new System.NotImplementedException();
    }
}
