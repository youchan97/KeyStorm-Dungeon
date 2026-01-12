using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static ConstValue;

public class Player : Character
{
    CharacterStateManager<Player> playerStateManager;
    GameManager gameManager;
    AudioManager audioManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerData data;
    [SerializeField] PlayerLimitData limitData;
    [SerializeField] PlayerSkill playerSkill;
    PlayerRunData playerRunData;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Animator anim;
    [SerializeField] Sprite bullet;
    [SerializeField] Sprite sBullet;
    [SerializeField] LayerMask itemLayer;
    [SerializeField] float magnetSpeed;
    [SerializeField] float magnetRangeMargin;
    [SerializeField] EffectData stepEffect;
    Vector3 prevPos;
    float moveDistance;
    [SerializeField] float stepDistance;
    [SerializeField] float stepSize;
    [SerializeField] Transform foot;


    bool isMove;
    bool isInvincible;
    bool isDashing;

    public event Action OnDie;

    #region Property
    public PlayerAttack PlayerAttack { get => playerAttack; private set=> playerAttack = value; }
    public bool IsMove { get => isMove; private set => isMove = value; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDieState DieState { get; private set; }
    public PlayerController PlayerController { get => playerController; private set => playerController = value; }
    public PlayerEffectStat PlayerEffectStat { get; private set; }
    public Rigidbody2D PlayerRb { get => playerRb; private set => playerRb = value; }
    public Animator Anim { get => anim; private set => anim = value; }
    public Sprite Bullet { get => bullet;}
    public Sprite SBullet { get => sBullet;}
    public PlayerInventory Inventory { get => inventory;}
    public AudioManager AudioManager { get => audioManager; }
    public PlayerSkill PlayerSkill { get => playerSkill;}
    public GameSceneUI GameSceneUI { get; set; }

    public EffectPoolManager EffectPoolManager { get; set; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    #endregion

    protected override void Awake()
    {
        playerStateManager = new CharacterStateManager<Player>(this);
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
        InitPlayer();
    }

    protected override void Update()
    {
        playerStateManager.Update();
        if (playerStateManager.CurState == MoveState)
        {
            HandleMoveEffect();
        }
        else
        {
            ResetMoveEffect();
        }
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
        PlayerEffectStat = new PlayerEffectStat(playerRunData, limitData);
        Inventory.InitInventory(playerRunData.inventory);
        Inventory.runData = playerRunData.inventory;
    }

    void InitActions()
    {
        PlayerController.OnMove += PlayerMove;
        playerController.OnShoot += Shoot;
        playerController.OnBomb += Bomb;
        playerController.OnUseActiveItem += UseActiveItem;
        PlayerController.OnPause += PausePlayer;
    }

    void RemoveActions()
    {
        PlayerController.OnMove -= PlayerMove;
        playerController.OnShoot -= Shoot;
        playerController.OnBomb -= Bomb;
        playerController.OnUseActiveItem -= UseActiveItem;
        playerController.OnPause -= PausePlayer;
    }

    void PlayerMove()
    {
        IsMove = PlayerController.MoveVec != Vector2.zero;
    }

    void Shoot()
    {
        PlayerAttack.Shoot(playerController.KeyName);
        GameSceneUI.UpdateAmmo();
    }

    void Bomb()
    {
        PlayerAttack.HoldBomb();
        GameSceneUI.UpdateBomb();
    }

    void UseActiveItem()
    {
        if (inventory.activeItem == null) return;

        playerSkill.TrySkill(inventory.activeItem.skillType);
    }

    void PausePlayer()
    {
        if (gameManager.isPaused)
        {
            gameManager.Resume();
            playerController.EnableInput();
            GameSceneUI.CloseAllPopup();
        }
        else
        {
            gameManager.Pause();
            playerController.DisableInput();
            GameSceneUI.OpenOption();
        }
    }

    public override void Die()
    {
        playerStateManager.ChangeState(DieState);
        OnDie?.Invoke();
        RemoveActions();
    }

    public void GameOverCanvas()
    {
        GameSceneUI.GameOver();
    }

    public override void TakeDamage(float damage)
    {
        if (isInvincible) return;

        isInvincible = true;
        CharacterRunData character = playerRunData.character;
        character.currentHp = Mathf.Max(0, character.currentHp - damage);

        Hp = character.currentHp;
        GameSceneUI.HealthUI.SetHp(Hp);

        audioManager.PlayEffect(PlayerHurtSfx);
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
        GameSceneUI.HealthUI.SetHp(Hp);
    }

    public void UpdatePlayerData(ItemData data)
    {
        if (!data.isActiveItem)
        {
            playerRunData.ApplyItemStat(data, limitData);
            SyncPlayerStat(playerRunData);
        }
    }

    void SyncPlayerStat(PlayerRunData runData)
    {
        CharacterRunData characterRunData = runData.character;

        MaxHp = characterRunData.maxHp;
        Hp = Mathf.Clamp(characterRunData.currentHp, 0f, MaxHp);
        MoveSpeed = PlayerEffectStat.GetMoveSpeed;
        transform.localScale = new Vector3(PlayerEffectStat.GetScaleX, PlayerEffectStat.GetScaleY, transform.localScale.z);
        PlayerAttack.SyncPlayerAttackStat(runData);

        GameSceneUI.UpdateAmmo();
        GameSceneUI.HealthUI.SetMaxHp(MaxHp);
        GameSceneUI.HealthUI.SetHp(Hp);
        if(Hp <= 0)
        {
            Die();
        }
    }

    public void MagnetItems(Bounds bounds)
    {
        Vector2 boxSize = (Vector2)bounds.size + Vector2.one * magnetRangeMargin * 2f;
        Collider2D[] cols = Physics2D.OverlapBoxAll(bounds.center, boxSize, itemLayer);

        foreach(Collider2D col in cols)
        {
            GoldPickup gold = col.GetComponent<GoldPickup>();
            if (gold != null)
                gold.EnableMagnet(transform, magnetSpeed);
        }
    }

    void HandleMoveEffect()
    {
        Vector3 curPos = foot.position;
        float distance = Vector3.Distance(prevPos, curPos);

        moveDistance += distance;
        prevPos = curPos;

        if (moveDistance >= stepDistance)
        {
            SpawnEffect(curPos);
            moveDistance -= stepDistance;
        }
    }

    void ResetMoveEffect()
    {
        prevPos = transform.position;
        moveDistance = 0f;
    }

    void SpawnEffect(Vector3 curPos)
    {
        Effect effect = EffectPoolManager.GetObj();
        effect.transform.position = (curPos);
        effect.InitData(EffectPoolManager, stepEffect, Vector2.zero, stepSize);
    }
}
