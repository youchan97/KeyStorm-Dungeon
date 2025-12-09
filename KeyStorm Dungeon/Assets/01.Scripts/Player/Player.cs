using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] PlayerController playerController;
    CharacterStateManager<Player> playerStateManager;

    PlayerIdleState idleState;
    PlayerMoveState moveState;
    PlayerDieState dieState;

    bool isMove;

    protected override void Awake()
    {
        playerStateManager = new CharacterStateManager<Player>(this);
    }


    private void Start()
    {
        InitState();
    }

    protected override void InitState()
    {
        idleState = new PlayerIdleState(this, playerStateManager);
        moveState = new PlayerMoveState(this, playerStateManager);
        dieState = new PlayerDieState(this, playerStateManager);
        playerStateManager.ChangeState(idleState);
    }

    void InitActions()
    {
        playerController.OnMove += PlayerMove;
    }

    void RemoveActions()
    {
        playerController.OnMove -= PlayerMove;
    }

    void PlayerMove()
    {
        isMove = true;
    }

    public override void Die()
    {
        playerStateManager.ChangeState(dieState);
    }
}
