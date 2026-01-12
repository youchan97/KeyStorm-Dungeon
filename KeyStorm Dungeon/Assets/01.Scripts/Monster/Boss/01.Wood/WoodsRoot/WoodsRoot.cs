using UnityEngine;

public class WoodsRoot : MeleeMonster
{
    [SerializeField] private float attackRadius;

    [SerializeField] private float slashAttackCooldown;

    public float SlashAttackCooldown => slashAttackCooldown;

    private WoodsRootIdleState _idleState;
    private WoodsRootMoveState _moveState;
    private WoodsRootAttackState _attackState;
    private WoodsRootDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new WoodsRootIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new WoodsRootMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new WoodsRootAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new WoodsRootDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Start()
    {
        PlayerGO = player.gameObject;

        if (PlayerGO == null)
        {
            Debug.LogError("Monster : Player GameObject를 찾을 수 없음");
        }
        else
        {
            PlayerTransform = PlayerGO.transform;
            player.OnDie += OnStopChase;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    // 스폰 애니메이션에서 호출
    public void OnSpawnFinish()
    {
        MonsterStateManager.ChangeState(CreateIdleState());
    }

    // 어택 애니메이션에서 호출
    public void OnAttack()
    {
        SlashAttack(transform.position, attackRadius);
    }

    public void OnAttackFinish()
    {
        MonsterStateManager.ChangeState(CreateIdleState());
    }

    public void SlashAttack(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(Damage);
            }
        }
    }
}
