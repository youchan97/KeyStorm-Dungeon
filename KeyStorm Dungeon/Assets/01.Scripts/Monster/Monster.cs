using System;
using UnityEngine;

public abstract class Monster : Character
{
    public CharacterStateManager<Monster> MonsterStateManager { get; protected set; }
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] protected SpriteRenderer monsterSpriteRenderer;

    public MonsterData MonsterData => _monsterData;
    private Rigidbody2D monsterRb;
    public Rigidbody2D MonsterRb => monsterRb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    public GameObject PlayerGO {  get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Player player { get; protected set; }

    [HideInInspector] public float CurrentAttackCooldown { get; protected set; }

    public abstract CharacterState<Monster> CreateIdleState();
    public abstract CharacterState<Monster> CreateMoveState();
    public abstract CharacterState<Monster> CreateAttackState();
    public abstract CharacterState<Monster> CreateDieState();

    public event Action OnMonsterDied;
    
    public Room MyRoom { get; set; }

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

        if (monsterSpriteRenderer == null)
        {
            monsterSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    protected virtual void Start()
    {
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
        //PlayerGO = PlayerSpawner.playerObj;
        
        if (PlayerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
            player = PlayerGO.GetComponent<Player>();
            PlayerTransform = PlayerGO.transform;
            player.OnDie += OnStopChase;
            MonsterStateManager.ChangeState(CreateIdleState());
        }

    }

    private void OnDisable()
    {
        OnMonsterDied = null;
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
        if (MyRoom != null)
        {
            MyRoom.RemoveMonster(this);

            if (MonsterData != null && MonsterData.tier == MonsterTier.Boss)
            {
                MyRoom.StageClear(transform.position);
            }
        }
        MonsterStateManager.ChangeState(CreateDieState());
    }

    public void SetAttackTarget(Player player)
    {
        this.player = player;
    }

    // 몬스터가 플레이어 위치에 따라 스프라이트 반전에서 현재 이동방향에 따라 반전하도록 하는 것이 올바름
    public virtual void FlipSprite(float moveDirectionX)
    {
        if (monsterSpriteRenderer == null) return;

        if (moveDirectionX < 0)
        {
            monsterSpriteRenderer.flipX = false;
        }
        else if(moveDirectionX > 0)
        {
            monsterSpriteRenderer.flipX = true;
        }
    }

    public virtual void FlipSpriteAttack(Transform playerTransform)
    {
        if (playerTransform == null && monsterSpriteRenderer == null) return;

        if (playerTransform.position.x < monsterRb.position.x)
        {
            monsterSpriteRenderer.flipX = false;
        }
        else
        {
            monsterSpriteRenderer.flipX = true;
        }
    }

    // 죽음을 알리는 이벤트 메서드
    public void InvokeOnMonsterDied()
    {
        OnMonsterDied?.Invoke();
    }

    public void SetMyRoom(Room room)
    {
        MyRoom = room;
    }

    public void ChangeStateToPlayerDied()
    {
        MonsterStateManager.ChangeState(CreateIdleState());
    }

    void OnStopChase() => PlayerGO = null;
}
