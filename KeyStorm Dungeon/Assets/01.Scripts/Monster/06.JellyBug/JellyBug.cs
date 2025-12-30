using UnityEngine;

public class JellyBug : MeleeMonster
{
    [Header("젤리벌레 설정")]
    [SerializeField] private float mitosisSpawnOffsetX;
    [SerializeField] private GameObject jellyBugMitosisPrefab;
    [SerializeField] private string poisonFieldPoolName = "PoisonField";
    [SerializeField] private float poisonFieldDuration = 3f;

    [Header("몬스터 감지")]
    [SerializeField] private LayerMask otherMonsterLayer;
    [SerializeField] private float otherMonsterDetectionDistance;

    public Vector2 CurrentMoveDirection { get; set; } = Vector2.zero;
    private CapsuleCollider2D currentMonsterCollider;

    public LayerMask OtherMonsterLayer => otherMonsterLayer;
    public float OtherMonsterDetectionDistance => otherMonsterDetectionDistance;
    public CapsuleCollider2D GetCurrentMonsterCollider() => currentMonsterCollider;

    private JellyBugIdleState _idleState;
    private JellyBugMoveState _moveState;
    private JellyBugAttackState _attackState;
    private JellyBugDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new JellyBugIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new JellyBugMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new JellyBugAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new JellyBugDieState(this, MonsterStateManager);
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

    public void SpawnMitosis(Vector3 spawnPosition)
    {
        GameObject mitosisGO = Instantiate(jellyBugMitosisPrefab, spawnPosition, Quaternion.identity);
        JellyBugMitosis newMitosis = mitosisGO.GetComponent<JellyBugMitosis>();

        if ( newMitosis != null)
        {
            newMitosis.SetMyRoom(MyRoom);
            MyRoom?.AddMonster(newMitosis);
        }
    }

    public void OnDieEffect()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, -0.3f, 0);

        SpawnMitosis(spawnPosition + Vector3.left * mitosisSpawnOffsetX);
        SpawnMitosis(spawnPosition + Vector3.right * mitosisSpawnOffsetX);

        GameObject poisonFieldObject = ObjectPoolManager.Instance.GetObject(poisonFieldPoolName, spawnPosition, Quaternion.identity);

        if (poisonFieldObject != null)
        {
            PoisonField poisonField = poisonFieldObject.GetComponent<PoisonField>();

            if (poisonField != null)
            {
                poisonField.Initialize(poisonFieldDuration);
            }
            else
            {
                Debug.LogError($"풀매니저에 독장판 컴포넌트가 없음");
                ObjectPoolManager.Instance.ReturnObject(poisonFieldObject, poisonFieldPoolName);
            }
        }
        else
        {
            Debug.LogError($"독장판 풀에서 오브젝트를 가져오지못함");
        }
    }
}
