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

    public WoodMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        wood = character as Wood;
    }

    public override void EnterState()
    {
        rb = wood.MonsterRb;
        animator = wood.Animator;
        playerTransform = wood.PlayerTransform;
        currentMoveDelay = 0f;
        isMoving = false;
        currentFootStep = 0;
        targetFootStep = Random.Range(wood.MinFootStepCount, wood.MaxFootStepCount + 1);
        wood.OnTakeOneStepAnimation += FinishedOneFootStep;
    }

    public override void FixedUpdateState()
    {
        if (wood.PlayerTransform == null || wood.PlayerGO == null) return;

        if (currentFootStep >= targetFootStep)
        {
            rb.velocity = Vector2.zero;
            stateManager.ChangeState(wood.CreateAttackState());
            return;
        }

        currentMoveDelay -= Time.fixedDeltaTime;

        if (currentMoveDelay <= 0)
        {
            WoodMove();
            if (!isMoving)
            {
                animator.SetTrigger(MoveAnim);
                isMoving = true;
            }
        }
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.ResetTrigger(MoveAnim);
        }

        wood.OnTakeOneStepAnimation -= FinishedOneFootStep;
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    public void WoodMove()
    {
        Vector2 direction = (playerTransform.position - wood.transform.position).normalized;
        animator.SetFloat(AxisX, direction.x);
        animator.SetFloat(AxisY, direction.y);

        rb.velocity = direction * wood.MonsterData.characterData.moveSpeed;
    }

    private void FinishedOneFootStep()
    {
        rb.velocity = Vector2.zero;
        currentFootStep++;
        currentMoveDelay = wood.MoveDelay;
        isMoving = false;
    }
}
