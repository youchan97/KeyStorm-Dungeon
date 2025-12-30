using UnityEngine;
using static ConstValue;

public class BQueenMoveState : MonsterMoveState
{
    private BQueen bQueen;

    private Vector2 moveDirection;
    private float currentMoveTime;

    public BQueenMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        bQueen = character as BQueen;
    }

    public override void EnterState()
    {
        rb = bQueen.MonsterRb;
        animator = bQueen.Animator;

        moveDirection = Random.insideUnitCircle.normalized;
        currentMoveTime = bQueen.MoveTime;

        UpdateAnimation();
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (bQueen.PlayerGO == null)
        {
            bQueen.ChangeStateToPlayerDied();
            return;
        }

        if (character.isKnockBack) return;

        currentMoveTime -= Time.deltaTime;

        if (currentMoveTime <= 0)
        {
            stateManager.ChangeState(bQueen.CreateAttackState());
            return;
        }

        rb.velocity = moveDirection * bQueen.MoveSpeed;
        UpdateAnimation();
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetFloat(AxisX, moveDirection.x);
        animator.SetFloat(AxisY, moveDirection.y);
    }
}
