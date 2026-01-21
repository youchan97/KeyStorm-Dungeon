using UnityEngine;

public class SplitRuinsSlime : RuinsSlime
{
    [Header("2차 분열 사원 슬라임")]
    [SerializeField] private TwiceSplitRuinsSlime twiceSplitRuinsSlime;

    #region 도약 패턴에서 사용할 방향 백터
    protected override Vector2[] DiveBulletDirections => new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };
    #endregion

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

    protected override void SpawnSplitSlime(Vector3 spawnPosition)
    {
        TwiceSplitRuinsSlime newSlime = Instantiate(twiceSplitRuinsSlime, spawnPosition, Quaternion.identity);

        if (newSlime != null)
        {
            newSlime.SetMyRoom(MyRoom);
            MyRoom?.AddMonster(newSlime);
        }
    }
}
