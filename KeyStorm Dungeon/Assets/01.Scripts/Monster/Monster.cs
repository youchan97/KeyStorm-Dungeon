using System;
using System.Collections;
using UnityEngine;
using static ConstValue;

public abstract class Monster : Character
{
    public CharacterStateManager<Monster> MonsterStateManager { get; protected set; }
    [SerializeField] private MonsterData monsterData;
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

    [Header("픽업 아이템 드롭 허용")]
    [SerializeField] protected bool itemDropSwitch = true;

    protected AudioManager audioManager;
    protected Rigidbody2D monsterRb;
    protected float currentAttackCooldown;

    #region 속성
    public MonsterData MonsterData => monsterData;
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
    public bool ItemDropSwitch => itemDropSwitch;
    public float CurrentAttackCooldown => currentAttackCooldown;
    #endregion

    public abstract CharacterState<Monster> CreateIdleState();
    public abstract CharacterState<Monster> CreateMoveState();
    public abstract CharacterState<Monster> CreateAttackState();
    public abstract CharacterState<Monster> CreateDieState();

    // 몬스터 이벤트
    public event Action OnMonsterDied;
    public event Action OnTakeDamage;
    
    // 몬스터가 소환될 방
    public Room MyRoom { get; set; }

    // 넉백 아이템 관련 속성
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

        if (monsterData != null)
        {
            // 몬스터 데이터 적용
            InitCharData(monsterData.characterData);
            currentAttackCooldown = 0f;
            transform.localScale = new Vector3(monsterData.xScale, monsterData.yScale, 1f);
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
            // player가 죽었을 시 사용할 이벤트 구독
            player.OnDie += OnStopChase;
            // Idle상태로 시작
            MonsterStateManager.ChangeState(CreateIdleState());
        }
    }

    protected virtual void OnDisable()
    {
        OnMonsterDied = null;
    }

    protected override void Update()
    {
        // FSM 기반 상태별 Update 실행
        MonsterStateManager.Update();

        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // FSM 기반 상태별 FixedUpdate 실행
        MonsterStateManager.FixedUpdate();
    }

    public override void Attack(Character character)
    {
        base.Attack(character);
    }

    // 공격 쿨타임을 리셋
    public void ResetAttackCooldown()
    {
        if (monsterData != null)
        {
            currentAttackCooldown = monsterData.attackSpeed;
        }
        else
        {
            Debug.LogWarning("MonsterData가 할당되지 않아 쿨타임을 초기화할 수 없음");
        }
    }

    // 데미지를 몬스터 체력에 적용하는 메서드
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        // 피격 애니메이션 실행
        animator.SetTrigger(HurtAnim);
        // 보스 체력바 UI에 감소된 체력 적용
        if (MonsterData.tier == MonsterTier.Boss)
        {
            OnTakeDamage?.Invoke();
        }
    }

    public override void Die()
    {
        base.Die();
        // 이벤트에 구독된 메서드 실행
        OnMonsterDied?.Invoke();
        // Die 상태로 변경
        MonsterStateManager.ChangeState(CreateDieState());

        if (MyRoom != null)
        {
            // 몬스터가 속한 방의 몬스터 리스트에서 제거 (방에서의 몬스터 생사여부 확인용)
            MyRoom.RemoveMonster(this);
        }

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        if (hook != null)
        {
            if (MonsterData != null && MonsterData.tier == MonsterTier.Boss)
            {
                hook.ReportBossKill();
            }

            hook.ReportEnemyKill();
        }
    }

    // 현재 이동방향에 따라 스프라이트를 반전 (왼쪽 방향 스프라이트만 존재시)
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

    // 몬스터의 공격 방향에 따라 스프라이트를 반전 (왼쪽 방향 스프라이트만 존재시)
    public virtual void FlipSpriteAttack(Transform playerTransform)
    {
        if (playerTransform == null && monsterSpriteRenderer == null) return;

        //
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

    // 방을 몬스터의 방으로 지정한 뒤 해당 방의 플레이어를 가져오는 메서드
    public void SetMyRoom(Room room)
    {
        MyRoom = room;
        player = room.Player;
    }

    // 플레이어가 죽었을 때 Idle 상태로 변경하는 메서드
    public void ChangeStateToPlayerDied()
    {
        MonsterStateManager.ChangeState(CreateIdleState());
    }

    protected void OnStopChase() => PlayerGO = null;

    // 넉백 아이템 사용시 적용되는 메서드
    public void ApplyKnockBack(Vector2 dir, float force, float duration)
    {
        if(knockBackCo != null)
        {
            StopCoroutine(knockBackCo);
            knockBackCo = null;
        }
        // 넉백 코루틴 실행
        knockBackCo = StartCoroutine(KnockBackRoutine(dir, force, duration));
    }

    // 넉백 코루틴
    IEnumerator KnockBackRoutine(Vector2 dir, float force, float duration)
    {
        // 넉백 중임을 알림 - 움직임 Update 제어용
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

    // 소환하는 몬스터가 있을 경우 벽 너머에 소환하는 것을 방지
    protected bool IsSpawnPositionValid(Vector3 position, float radius, LayerMask layerMask)
    {
        // 몬스터의 위치와 반경을 검사하여 주변에 벽이 존재할 경우 null 리턴
        return Physics2D.OverlapCircle(position, radius, layerMask) == null;
    }
}
