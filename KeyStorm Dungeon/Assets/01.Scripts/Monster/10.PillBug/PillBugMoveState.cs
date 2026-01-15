using UnityEngine;
using static ConstValue;

public class PillBugMoveState : MonsterMoveState
{
    private PillBug pillBug;

    public PillBugMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        pillBug = character as PillBug;
    }

    public override void EnterState()
    {
        rb = pillBug.MonsterRb;
        animator = pillBug.Animator;
        playerTransform = pillBug.PlayerTransform;

        pillBug.ChangeIsChase(true);
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (pillBug.PlayerGO == null)
        {
            pillBug.ChangeStateToPlayerDied();
            return;
        }

        if (pillBug.isKnockBack || pillBug.IsReaction) return;

        rb.velocity = pillBug.CurrentMoveDirection * pillBug.MoveSpeed * pillBug.MoveSpeedMultiple;
        
        animator.SetFloat(AxisX, rb.velocity.x);
        animator.SetFloat(AxisY, rb.velocity.y);
    }

    public override void ExitState()
    {
        pillBug.ChangeIsChase(false);
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }
}
