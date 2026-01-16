using UnityEngine;

public class Mqt : MeleeMonster
{
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;
    [SerializeField] private float attackedIdleTime;
    [SerializeField] private float attackedMoveTime;
    [SerializeField] private float attackedMoveSpeedMultiple;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private float bulletLifeTime;

    private bool attackedPlayer;

    #region 속성
    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float AttackedIdleTime => attackedIdleTime;
    public float AttackedMoveTime => attackedMoveTime;
    public float AttackedMoveSpeedMultiple => attackedMoveSpeedMultiple;
    public bool AttackedPlayer => attackedPlayer;
    #endregion

    #region 애니메이션
    private const string AttackedMoveAnim = "AttackedMove";
    #endregion

    private MqtIdleState _idleState;
    private MqtMoveState _moveState;
    private MqtAttackState _attackState;
    private MqtDieState _dieState;

    private Vector2[] bulletDirection = new Vector2[]
    {
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
    };

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new MqtIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new MqtMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new MqtAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MqtDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
        attackedPlayer = false;

        if (attackPoolManager == null)
        {
            attackPoolManager = FindObjectOfType<AttackPoolManager>();
        }
    }

    protected override void ContactPlayer(Collision2D collision)
    {
        if (CurrentAttackCooldown <= 0f)
        {
            if (((1 << collision.gameObject.layer) & playerLayer.value) > 0)
            {
                Player player = collision.gameObject.GetComponent<Player>();

                if (player != null)
                {
                    Attack(player);
                    if (!attackedPlayer)
                    {
                        attackedPlayer = true;
                        Animator.SetBool(AttackedMoveAnim, true);
                    }
                    ResetAttackCooldown();
                }
            }
        }
    }

    public void OnSplashBullet()
    {
        if (attackPoolManager == null) return;
        
        foreach (var direction in bulletDirection)
        {
            AttackObj pooledAttackObject = AttackPoolManager.GetObj();

            pooledAttackObject.transform.position = shootPoint.position;
            pooledAttackObject.transform.rotation = Quaternion.identity;
            pooledAttackObject.InitData(bulletSprite, Damage, direction, MonsterData.shotSpeed, bulletLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius, null);
        }
    }
}
