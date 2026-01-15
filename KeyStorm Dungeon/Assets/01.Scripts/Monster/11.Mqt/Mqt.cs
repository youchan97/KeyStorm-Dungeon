using UnityEngine;

public class Mqt : MeleeMonster
{
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;
    [SerializeField] private float attackedIdleTime;
    [SerializeField] private float attackedMoveSpeed;
    private bool attackedPlayer;
    
    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float AttackedIdleTime => attackedIdleTime;
    public float AttackedMoveSpeed => attackedMoveSpeed;
    public bool AttackedPlayer => attackedPlayer;

    private MqtIdleState _idleState;
    private MqtMoveState _moveState;
    private MqtAttackState _attackState;
    private MqtDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new MqtIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new MqtMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new MqtAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MqtDieState(this, MonsterStateManager);
        return _dieState;
    }
}
