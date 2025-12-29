using UnityEngine;

public class NeedlehogAttackState : MonsterAttackState
{
    private Needlehog needlehog;

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

        animator.SetTrigger("IsCrouch");
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
            animator.SetTrigger("IsAttack");
            needlehog.ResetAttackCooldown();
        }
        
    }

    public override void ExitState()
    {
        if (animator != null)
        {
            animator.ResetTrigger("IsAttack");
        }
    }
}
