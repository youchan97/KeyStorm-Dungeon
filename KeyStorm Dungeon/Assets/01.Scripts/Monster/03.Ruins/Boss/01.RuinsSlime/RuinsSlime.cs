using UnityEngine;

public class RuinsSlime : Slime
{
    [Header("1차 분열 사원 슬라임")]
    [SerializeField] private SplitRuinsSlime splitRuinsSlime;
    [SerializeField] protected float splitSlimeSpawnOffsetX;

    private RuinsSlimeIdleState _idleState;
    private RuinsSlimeMoveState _moveState;
    private RuinsSlimeAttackState _attackState;
    private RuinsSlimeDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new RuinsSlimeIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new RuinsSlimeMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new RuinsSlimeAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new RuinsSlimeDieState(this, MonsterStateManager);
        return _dieState;
    }

    public override void Die()
    {
        base.Die();
        SpawnSplitSlimeFromDie();
    }

    protected virtual void SpawnSplitSlime(Vector3 spawnPosition)
    {
        SplitRuinsSlime newSlime = Instantiate(splitRuinsSlime, spawnPosition, Quaternion.identity);

        if (newSlime != null)
        {
            newSlime.SetMyRoom(MyRoom);
            MyRoom?.AddMonster(newSlime);
        }
    }

    protected void SpawnSplitSlimeFromDie()
    {
        Vector3 spawnPosition = transform.position;

        SpawnSplitSlime(spawnPosition + Vector3.left * splitSlimeSpawnOffsetX);
        SpawnSplitSlime(spawnPosition + Vector3.right * splitSlimeSpawnOffsetX);
    }
}
