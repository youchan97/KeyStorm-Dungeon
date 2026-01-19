using UnityEngine;
using static ConstValue;

public class NeedlehogAttackState : MonsterAttackState
{
    private Needlehog needlehog;

    #region 애니메이션
    private const string CrouchAnim = "IsCrouch";
    #endregion

    public NeedlehogAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        needlehog = character as Needlehog;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        animator.SetTrigger(CrouchAnim);
    }

    public override void UpdateState()
    {
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        if (needlehog.CurrentAttackCooldown <= 0)
        {
            animator.SetTrigger(AttackAnim);
            needlehog.ResetAttackCooldown();
        }
        
    }

    public override void ExitState()
    {
        if (animator != null)
        {
            animator.ResetTrigger(AttackAnim);
        }
    }
}
