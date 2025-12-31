using UnityEngine;

public class JellyBugMitosis : MeleeMonster
{
    [Header("독 장판 생성")]
    [SerializeField] private Vector3 poisonSpawnPointOffset;
    [SerializeField] private float poisonCooldown;
    [SerializeField] private string poisonFieldPoolName = "PoisonField";
    [SerializeField] private float poisonFieldDuration = 3f;

    [Header("몬스터 감지")]
    [SerializeField] private LayerMask otherMonsterLayer;

    [Header("독장판 크기 설정")]
    [SerializeField] private Vector3 poisonFieldLocalScale;

    public Vector2 CurrentMoveDirection { get; set; } = Vector2.zero;

    public float PoisonCooldown => poisonCooldown;
    public LayerMask OtherMonsterLayer => otherMonsterLayer;
    public Vector3 PoisonSpawnPointOffset => poisonSpawnPointOffset;

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
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject == gameObject) return;

        ContactEnemy(collision);
    }

    public void SpawnPoisonField(Vector3 spawnPosition)
    {
        GameObject poisonFieldObject = ObjectPoolManager.Instance.GetObject(poisonFieldPoolName, spawnPosition, Quaternion.identity);

        if (poisonFieldObject != null)
        {
            PoisonField poisonField = poisonFieldObject.GetComponent<PoisonField>();

            if (poisonField != null)
            {
                poisonFieldObject.transform.localScale = poisonFieldLocalScale;
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

    private void ContactEnemy(Collider2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if (((1 << currentGameObject.layer) & otherMonsterLayer) != 0)
        {
            MonsterRb.velocity = Vector2.zero;
            MonsterStateManager.ChangeState(CreateIdleState());
        }
    }
}
