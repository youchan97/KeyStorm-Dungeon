using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    [SerializeField] private MonsterData _monsterData;
    public MonsterData MonsterData => _monsterData;
    protected CharacterStateManager<Monster> monsterStateManager;


    [HideInInspector] public float CurrentAttackCooldown { get; private set; }


    protected override void Awake()
    {
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
        /*GameObject playerGO = GameManager.Instance.player;
        
        if (playerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
            Transform playerTrasnfrom = playerGO.transform;
            monsterStateManager.ChangeState(new MonsterMoveState(this, playerTrasnfrom));
        }*/

    }

    protected override void Update()
    {
        monsterStateManager?.Update();

        if (CurrentAttackCooldown > 0f)
        {
            CurrentAttackCooldown -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        monsterStateManager?.FixedUpdate();
    }

    public override void Attack(Character character)
    {
        base.Attack(character);
    }

    protected void ResetAttackCooldown()
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
        monsterStateManager.ChangeState(dieState);
    }
}
