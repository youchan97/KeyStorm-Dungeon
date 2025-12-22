using UnityEngine;

public class NeedlehogAttackState : MonsterAttackState
{
    private Needlehog needlehog;

    public NeedlehogAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.needlehog = character as Needlehog;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        needlehog.Animator.SetTrigger("IsCrouch");
    }

    public override void UpdateState()
    {
        if (character.player.Hp <= 0)
        {
            character.ChangeStateToPlayerDied();
        }

        if (needlehog.CurrentAttackCooldown <= 0)
        {
            needlehog.Animator.SetTrigger("IsAttack");
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
