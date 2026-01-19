using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWorker : MeleeMonster
{
    private BQueen assignedBQueen;
    public BQueen AssignedBQueen => assignedBQueen;

    private bool _isSpawnedImpulseActive = false;
    private float _spawnImpulseRemainingTime;

    public bool IsSpawnedImpulseActive => _isSpawnedImpulseActive;
    public float SpawnImpulseRemainingTime => _spawnImpulseRemainingTime;

    private BWorkerIdleState _idleState;
    private BWorkerMoveState _moveState;
    private BWorkerAttackState _attackState;
    private BWorkerDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new BWorkerIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new BWorkerMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new BWorkerAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new BWorkerDieState(this, MonsterStateManager);
        return _dieState;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    public void SetAssignedBQueen(BQueen bQueen, float impulseDuration)
    {
        assignedBQueen = bQueen;
        _isSpawnedImpulseActive = true;
        _spawnImpulseRemainingTime = impulseDuration;
    }

    public void DecrementSpawnImpulseTime(float time)
    {
        if (_isSpawnedImpulseActive)
        {
            _spawnImpulseRemainingTime -= time;
            if (_spawnImpulseRemainingTime <= 0)
            {
                _isSpawnedImpulseActive = false;
            }
        }
    }

    public override void Die()
    {
        base.Die();
        if (assignedBQueen != null)
        {
            assignedBQueen.RemoveWorker(this);
        }
    }
}
