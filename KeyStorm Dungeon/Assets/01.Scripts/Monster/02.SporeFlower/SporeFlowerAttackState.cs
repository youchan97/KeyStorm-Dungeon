using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeFlowerAttackState : MonsterAttackState
{
    private SporeFlower sporeFlower;

    public SporeFlowerAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.sporeFlower = character as SporeFlower;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        animator.SetTrigger("IsAttack");
        sporeFlower.ResetAttackCooldown();
    }

    public override void UpdateState()
    {
        if(character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        if (sporeFlower.CurrentAttackCooldown <= 0)
        {
            stateManager.ChangeState(sporeFlower.CreateIdleState());
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
