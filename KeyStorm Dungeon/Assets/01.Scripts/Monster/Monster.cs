using System;
using System.Collections;
using UnityEngine;
using static ConstValue;

public abstract class Monster : Character
{
    public CharacterStateManager<Monster> MonsterStateManager { get; protected set; }
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] protected SpriteRenderer monsterSpriteRenderer;
    [SerializeField] private Animator animator;

    [Header("벽, 장애물 레이어")]
    [SerializeField] protected LayerMask obstacleLayer;

    [Header("소환몹 소환 자리 유효성 검사")]
    [SerializeField] protected float spawnCheckRadius;

    [Header("플레이어 레이어")]
    [SerializeField] protected LayerMask playerLayer;

    [Header("카메라 흔들림 연출")]
    [SerializeField] protected float shakePower;
    [SerializeField] protected float shakeDuration;

    protected AudioManager audioManager;

    public MonsterData MonsterData => _monsterData;
    private Rigidbody2D monsterRb;
    public Rigidbody2D MonsterRb => monsterRb;
    public Animator Animator => animator;
    public LayerMask ObstacleLayer => obstacleLayer;
    public LayerMask PlayerLayer => playerLayer;
    public AudioManager AudioManager => audioManager;
    public float ShakePower => shakePower;
    public float ShakeDuration => shakeDuration;
    public GameObject PlayerGO {  get; protected set; }
    public Transform PlayerTransform { get; protected set; }
    public Player player { get; protected set; }

    protected float currentAttackCooldown;
    public float CurrentAttackCooldown => currentAttackCooldown;

    public abstract CharacterState<Monster> CreateIdleState();
    public abstract CharacterState<Monster> CreateMoveState();
    public abstract CharacterState<Monster> CreateAttackState();
    public abstract CharacterState<Monster> CreateDieState();

    public event Action OnMonsterDied;
    public event Action OnTakeDamage;
    
    public Room MyRoom { get; set; }

    public bool isKnockBack;
    Coroutine knockBackCo;

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
            currentAttackCooldown = 0f;
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
        audioManager = AudioManager.Instance;

        PlayerGO = player.gameObject;
        
        if (PlayerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
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

        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
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
            currentAttackCooldown = _monsterData.attackSpeed;
        }
        else
        {
            Debug.LogWarning("MonsterData가 할당되지 않아 쿨타임을 초기화할 수 없음");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        animator.SetTrigger(HurtAnim);
        if (MonsterData.tier == MonsterTier.Boss)
        {
            OnTakeDamage?.Invoke();
        }
    }

    public override void Die()
    {
        base.Die();
        
        MonsterStateManager.ChangeState(CreateDieState());

        if (MyRoom != null)
        {
            MyRoom.RemoveMonster(this);

            /*if (MonsterData != null && MonsterData.tier == MonsterTier.Boss)
            {
                MyRoom.StageClear(transform.position);
            }*/
        }

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        if (hook != null)
        {
            hook.ReportEnemyKill();
        }

        Destroy(gameObject);
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
        player = room.Player;
    }

    public void ChangeStateToPlayerDied()
    {
        MonsterStateManager.ChangeState(CreateIdleState());
    }

    protected void OnStopChase() => PlayerGO = null;

    public void ApplyKnockBack(Vector2 dir, float force, float duration)
    {
        if(knockBackCo != null)
        {
            StopCoroutine(knockBackCo);
            knockBackCo = null;
        }
        knockBackCo = StartCoroutine(KnockBackRoutine(dir, force, duration));
    }

    IEnumerator KnockBackRoutine(Vector2 dir, float force, float duration)
    {
        isKnockBack = true;
        float originDrag = MonsterRb.drag;
        MonsterRb.drag = 0f;

        MonsterRb.velocity = Vector2.zero;
        MonsterRb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        MonsterRb.drag = originDrag;
        isKnockBack = false;
        knockBackCo = null;
    }

    protected bool IsSpawnPositionValid(Vector3 position, float radius, LayerMask layerMask)
    {
        return Physics2D.OverlapCircle(position, radius, layerMask) == null;
    }
}
