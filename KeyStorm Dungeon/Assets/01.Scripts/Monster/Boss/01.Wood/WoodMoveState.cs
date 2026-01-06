using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class WoodMoveState : MonsterMoveState
{
    private Wood wood;
    private float currentMoveDelay;
    private bool isMoving;

    private int currentFootStep; // 현재 걸음 수
    private int targetFootStep; // 목표 걸음 수

    private float distanceToPlayer;

    public WoodMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        wood = character as Wood;
    }

    public override void EnterState()
    {
        rb = wood.MonsterRb;
        animator = wood.Animator;
        playerTransform = wood.PlayerTransform;
        currentMoveDelay = wood.MoveDelay;
        isMoving = false;
        targetFootStep = Random.Range(wood.MinFootStepCount, wood.MaxFootStepCount + 1);
    }

    public override void FixedUpdateState()
    {
        if (character.PlayerTransform == null || character.PlayerGO == null) return;

        if (currentFootStep >= targetFootStep)
        {
            stateManager.ChangeState(wood.CreateAttackState());
            return;
        }

        currentMoveDelay -= Time.fixedDeltaTime;

        if (currentMoveDelay <= 0 && !isMoving)
        {
            animator.SetTrigger(MoveAnim);
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

    public void OnMove()
    {

    }
}
