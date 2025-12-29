using System;
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
    AudioManager audioManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerData data;
    PlayerRunData playerRunData;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Animator anim;
    [SerializeField] Sprite bullet;
    [SerializeField] Sprite sBullet;
    [SerializeField] LayerMask itemLayer;
    [SerializeField] float magnetSpeed;
    [SerializeField] float magnetRangeMargin;

    bool isMove;
    bool isInvincible;

    public event Action OnDie;

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
    public AudioManager AudioManager { get => audioManager; }
    #endregion

    protected override void Awake()
    {
        playerStateManager = new CharacterStateManager<Player>(this);
        PlayerAttack = GetComponent<PlayerAttack>();
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
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
        OnDie = null;
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
        playerRunData = gameManager.PlayerRunData;
        InitCharRunData(playerRunData.character);
        transform.localScale = new Vector3(playerRunData.xScale, playerRunData.yScale, transform.localScale.z);
        PlayerAttack.InitPlayerAttack(playerRunData);
        Inventory.InitInventory(playerRunData.inventory);
        Inventory.runData = playerRunData.inventory;
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
        OnDie?.Invoke();
        RemoveActions();
    }

    public override void TakeDamage(int damage)
    {
        if (isInvincible) return;

        CharacterRunData character = playerRunData.character;
        character.currentHp = Mathf.Max(0, character.currentHp - damage);

        Hp = character.currentHp;

        if (Hp > 0)
        {
            anim.SetTrigger(HurtAnim);
        }
        else
            Die();
    }

    public void SetInvincible(bool isSet)
    {
        isInvincible = isSet;
    }

    public void Heal(int num)
    {
        playerRunData.character.Heal(num);
        Hp = playerRunData.character.currentHp;
    }

    public void UpdatePlayerData(ItemData data)
    {
        if (!data.isActiveItem)
        {
            playerRunData.ApplyItemStat(data);
            SyncPlayerStat(playerRunData);
        }
    }

    void SyncPlayerStat(PlayerRunData runData)
    {
        CharacterRunData characterRunData = runData.character;

        Hp = characterRunData.currentHp;
        MaxHp = characterRunData.maxHp;
        MoveSpeed = characterRunData.moveSpeed;

        PlayerAttack.SyncPlayerAttackStat(runData);
    }

    public void MagnetItems(Bounds bounds)
    {
        //float detectDis = Mathf.Max(bounds.extents.x + magnetRangeMargin, bounds.extents.y + magnetRangeMargin);
        Vector2 boxSize = (Vector2)bounds.size + Vector2.one * magnetRangeMargin * 2f;
        Collider2D[] cols = Physics2D.OverlapBoxAll(bounds.center, boxSize, itemLayer);

        foreach(Collider2D col in cols)
        {
            //if (!bounds.Intersects(col.bounds)) continue;

            GoldPickup gold = col.GetComponent<GoldPickup>();
            if (gold != null)
                gold.EnableMagnet(transform, magnetSpeed);
        }
    }
}
