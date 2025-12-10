using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    CharacterStateManager<Player> playerStateManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerData data;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Animator anim;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject sBullet;

    bool isMove;

    #region Property
    public PlayerAttack PlayerAttack { get; private set; }
    public bool IsMove { get => isMove; private set => isMove = value; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDieState DieState { get; private set; }
    public PlayerController PlayerController { get => playerController; private set => playerController = value; }
    public Rigidbody2D PlayerRb { get => playerRb; private set => playerRb = value; }
    public Animator Anim { get => anim; private set => anim = value; }
    public GameObject Bullet { get => bullet;}
    public GameObject SBullet { get => sBullet;}
    #endregion

    protected override void Awake()
    {
        playerStateManager = new CharacterStateManager<Player>(this);
        PlayerAttack = new PlayerAttack();
    }


    private void Start()
    {
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

    protected override void InitState()
    {
        IdleState = new PlayerIdleState(this, playerStateManager);
        MoveState = new PlayerMoveState(this, playerStateManager);
        DieState = new PlayerDieState(this, playerStateManager);
        playerStateManager.ChangeState(IdleState);
    }

    void InitData()
    {
        InitCharData(data.characterData);
        PlayerAttack.InitPlayerAttack(this, data);
    }

    void InitActions()
    {
        PlayerController.OnMove += PlayerMove;
        playerController.OnShoot += Shoot;
    }

    void RemoveActions()
    {
        PlayerController.OnMove -= PlayerMove;
        playerController.OnShoot -= Shoot;
    }

    void PlayerMove()
    {
        IsMove = PlayerController.MoveVec != Vector2.zero;
    }

    void Shoot()
    {
        PlayerAttack.Shoot(playerController.KeyName);
    }

    public override void Die()
    {
        playerStateManager.ChangeState(DieState);
    }
}
