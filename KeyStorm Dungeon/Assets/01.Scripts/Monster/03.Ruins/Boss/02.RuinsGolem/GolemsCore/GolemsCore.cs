using UnityEngine;

public class GolemsCore : RangerMonster
{
    [SerializeField] private RuinsGolem ruinsGolem;

    [Header("공통 수치")]
    [SerializeField] private float idleTime;

    [Header("핵 움직임 수치")]
    [SerializeField] private float minMoveDuration;
    [SerializeField] private float maxMoveDuration;

    public float IdleTime => idleTime;
    public float MinMoveDuration => minMoveDuration;
    public float MaxMoveDuration => maxMoveDuration;
    public Vector3 GolemCenterPosition => ruinsGolem.transform.position;

    private GolemsCoreIdleState _idleState;
    private GolemsCoreMoveState _moveState;
    private GolemsCoreDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new GolemsCoreIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new GolemsCoreMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        return null;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new GolemsCoreDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
