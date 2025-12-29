using UnityEngine;

public class JellyBugMitosis : MeleeMonster
{
    [Header("독 장판 생성")]
    [SerializeField] private GameObject poisonFieldPrefab;
    [SerializeField] private float poisonCooldown;

    [Header("몬스터 감지")]
    [SerializeField] private LayerMask otherMonsterLayer;
    [SerializeField] private float otherMonsterDetectionDistance;

    public Vector2 CurrentMoveDirection { get; set; } = Vector2.zero;
    private CapsuleCollider2D currentMonsterCollider;

    public float PoisonCooldown => poisonCooldown;
    public LayerMask OtherMonsterLayer => otherMonsterLayer;
    public float OtherMonsterDetectionDistance => otherMonsterDetectionDistance;
    public CapsuleCollider2D GetCurrentMonsterCollider() => currentMonsterCollider;

    private JellyBugMitosisIdleState _idleState;
    private JellyBugMitosisMoveState _moveState;
    private JellyBugMitosisAttackState _attackState;
    private JellyBugMitosisDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new JellyBugMitosisIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new JellyBugMitosisMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new JellyBugMitosisAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new JellyBugMitosisDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
        currentMonsterCollider = GetComponent<CapsuleCollider2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    public void SpawnPoisonField(Vector3 spawnPosition)
    {
        GameObject poisonFieldObject = Instantiate(poisonFieldPrefab, spawnPosition, Quaternion.identity);
        poisonFieldObject.transform.localScale = new Vector3(0.5f, 0.3f, 1f);
    }
}
