using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MqtMoveState : MonsterMoveState
{
    private Mqt mqt;
    private float currentMoveTime;
    private bool isMove;

    public MqtMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        mqt = character as Mqt;
    }

    public override void EnterState()
    {
        rb = mqt.MonsterRb;
        animator = mqt.Animator;
        playerTransform = mqt.PlayerTransform;

        currentMoveTime = mqt.MoveTime;
        isMove = false;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (mqt.PlayerGO == null)
        {
            mqt.ChangeStateToPlayerDied();
            return;
        }

        if (character.isKnockBack) return;

        currentMoveTime -= Time.deltaTime;

        if (currentMoveTime <= 0)
        {
            
        }

        
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }


}
