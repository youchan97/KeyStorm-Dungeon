using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static ConstValue;
using static UnityEngine.Rendering.DebugUI;

public class Player : Character
{
    CharacterStateManager<Player> playerStateManager;
    GameManager gameManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerData data;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Animator anim;
    [SerializeField] Sprite bullet;
    [SerializeField] Sprite sBullet;
    [SerializeField] LayerMask itemLayer;

    bool isMove;


    #region Property
    public PlayerAttack PlayerAttack { get; private set; }
    public bool IsMove { get => isMove; private set => isMove = value; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDieState DieState { get; private set; }
    public PlayerData Data { get => data; }
    public PlayerController PlayerController { get => playerController; private set => playerController = value; }
    public Rigidbody2D PlayerRb { get => playerRb; private set => playerRb = value; }
    public Animator Anim { get => anim; private set => anim = value; }
    public Sprite Bullet { get => bullet;}
    public Sprite SBullet { get => sBullet;}
    public PlayerInventory Inventory { get => inventory;}
    #endregion

    protected override void Awake()
    {
        playerStateManager = new CharacterStateManager<Player>(this);
        PlayerAttack = GetComponent<PlayerAttack>();
        gameManager = GameManager.Instance;
        InitPlayer();
    }

    protected override void Update()
    {
        playerStateManager.Update();
    }

    protected override void FixedUpdate()
    {
        playerStateManager.FixedUpdate();
    }

    private void OnDisable()
    {
        RemoveActions();
    }

    void InitPlayer()
    {
        InitState();
        InitActions();
        InitData();
    }

    public void InitAttackPoolManager(AttackPoolManager attackPoolManager)
    {
        this.attackPoolManager = attackPoolManager;
    }

    protected override void InitState()
    {
        IdleState = new PlayerIdleState(this, playerStateManager);
        MoveState = new PlayerMoveState(this, playerStateManager);
        DieState = new PlayerDieState(this, playerStateManager);
        playerStateManager.ChangeState(IdleState);
    }

    void InitData()
    {
        PlayerRunData runData = gameManager.PlayerRunData;
        InitCharRunData(runData.character);
        transform.localScale = new Vector3(runData.xScale, runData.yScale, transform.localScale.z);
        PlayerAttack.InitPlayerAttack(runData);
    }

    void InitActions()
    {
        PlayerController.OnMove += PlayerMove;
        playerController.OnShoot += Shoot;
        playerController.OnBomb += Bomb;
    }

    void RemoveActions()
    {
        PlayerController.OnMove -= PlayerMove;
        playerController.OnShoot -= Shoot;
        playerController.OnBomb -= Bomb;
    }

    void PlayerMove()
    {
        IsMove = PlayerController.MoveVec != Vector2.zero;
    }

    void Shoot()
    {
        PlayerAttack.Shoot(playerController.KeyName);
    }

    void Bomb()
    {
        PlayerAttack.HoldBomb();
    }

    public override void Die()
    {
        playerStateManager.ChangeState(DieState);
        RemoveActions();
    }

    public override void TakeDamage(int damage)
    {
        CharacterRunData character = gameManager.PlayerRunData.character;
        character.currentHp = Mathf.Max(0, character.currentHp - damage);

        Hp = character.currentHp;

        if (Hp > 0)
            anim.SetTrigger(HurtAnim);
        else
            Die();
    }

    public void PlayerStatUpdate(ItemData data)
    {
        gameManager.PlayerRunData.ApplyItemStat(data);
        SyncPlayerStat();
    }

    void SyncPlayerStat()
    {
        PlayerRunData playerRundata = gameManager.PlayerRunData;
        CharacterRunData characterRunData = playerRundata.character;

        Hp = characterRunData.currentHp;
        MaxHp = characterRunData.maxHp;
        MoveSpeed = characterRunData.moveSpeed;

        PlayerAttack.SyncPlayerAttackStat(playerRundata);
    }

    public void MagnetItems(Bounds bounds)
    {
        float detectDis = Mathf.Max(bounds.extents.x, bounds.extents.y);
        Collider2D[] cols = Physics2D.OverlapCircleAll(bounds.center, detectDis, itemLayer);

        foreach(Collider2D col in cols)
        {
            if (!bounds.Contains(col.transform.position)) continue;

            //아이템의 타깃 설정 및 자석기능
        }
    }
}
