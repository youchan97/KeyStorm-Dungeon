using UnityEngine;

public abstract class Monster : Character
{
    public CharacterStateManager<Monster> MonsterStateManager { get; protected set; }
    [SerializeField] private MonsterData _monsterData;
    public MonsterData MonsterData => _monsterData;
    private Rigidbody2D monsterRb;
    public Rigidbody2D MonsterRb => monsterRb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    public GameObject playerGO {  get; private set; }
    public Transform playerTransform { get; private set; }
    public Player CurrentAttackTarget { get; protected set; }

    [HideInInspector] public float CurrentAttackCooldown { get; protected set; }

    public abstract CharacterState<Monster> CreateIdleState();
    public abstract CharacterState<Monster> CreateMoveState();
    public abstract CharacterState<Monster> CreateAttackState();
    public abstract CharacterState<Monster> CreateDieState();

    protected override void Awake()
    {
        if (monsterRb == null)
        {
            monsterRb = GetComponent<Rigidbody2D>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        MonsterStateManager = new CharacterStateManager<Monster>(this);

        if (_monsterData != null)
        {
            InitCharData(_monsterData.characterData);
            CurrentAttackCooldown = 0f;
            transform.localScale = new Vector3(_monsterData.xScale, _monsterData.yScale, 1f);
        }
        else
        {
            Debug.LogWarning("MonsterData가 할당되지 않음");
        }
    }

    private void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");


        //GameObject playerGO = GameManager.Instance.player;
        
        if (playerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
            playerTransform = playerGO.transform;
            MonsterStateManager.ChangeState(CreateIdleState());
        }

    }

    protected override void Update()
    {
        MonsterStateManager.Update();

        if (CurrentAttackCooldown > 0f)
        {
            CurrentAttackCooldown -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        MonsterStateManager.FixedUpdate();

        /*float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= MonsterData.detectRange)
        {
            IsMove = true;
        }
        else
        {
            IsMove = false;
        }*/
    }

    public override void Attack(Character character)
    {
        base.Attack(character);
    }

    public void ResetAttackCooldown()
    {
        if (_monsterData != null)
        {
            CurrentAttackCooldown = _monsterData.attackSpeed;
        }
        else
        {
            Debug.LogWarning("MonsterData가 할당되지 않아 쿨타임을 초기화할 수 없음");
        }
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
        MonsterStateManager.ChangeState(CreateDieState());
    }

    public void SetAttackTarget(Player player)
    {
        CurrentAttackTarget = player;
    }
}
