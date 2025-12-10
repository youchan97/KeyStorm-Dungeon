using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    protected CharacterStateManager<Monster> monsterStateManager;
    [SerializeField] private MonsterData _monsterData;
    public MonsterData MonsterData => _monsterData;
    private Rigidbody2D monsterRb;
    public Rigidbody2D MonsterRb => monsterRb;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    public MonsterIdleState IdleState { get; private set; }
    public MonsterMoveState MoveState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterDieState DieState { get; private set; }

    public GameObject playerGO {  get; private set; }
    public Transform playerTransform { get; private set; }
    public bool IsMove {  get; private set; }

    [HideInInspector] public float CurrentAttackCooldown { get; private set; }


    protected override void Awake()
    {
        if (monsterRb == null)
        {
            monsterRb = GetComponent<Rigidbody2D>();
        }
        monsterStateManager = new CharacterStateManager<Monster>(this);

        if (_monsterData != null)
        {
            InitCharData(_monsterData.characterData);
            CurrentAttackCooldown = 0f;
        }
        else
        {
            Debug.LogWarning("MonsterData가 할당되지 않음");
        }
    }

    private void Start()
    {
        IdleState = new MonsterIdleState(this, monsterStateManager);
        MoveState = new MonsterMoveState(this, monsterStateManager);
        AttackState = new MonsterAttackState(this, monsterStateManager);
        DieState = new MonsterDieState(this, monsterStateManager);

        playerGO = GameObject.FindGameObjectWithTag("Player");


        //GameObject playerGO = GameManager.Instance.player;
        
        if (playerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
            playerTransform = playerGO.transform;
            monsterStateManager.ChangeState(IdleState);
        }

    }

    protected override void Update()
    {
        monsterStateManager.Update();

        if (CurrentAttackCooldown > 0f)
        {
            CurrentAttackCooldown -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        monsterStateManager.FixedUpdate();

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= MonsterData.detectRange)
        {
            IsMove = true;
        }
        else
        {
            IsMove = false;
        }
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
        monsterStateManager.ChangeState(DieState);
    }
}
