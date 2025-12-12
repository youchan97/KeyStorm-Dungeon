using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMonsterAttackState : MonsterAttackState
{

    public RangerMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void UpdateState()
    {
        if (player == null)
        {
            stateManager.ChangeState(character.CreateIdleState());
            return;
        }

        character.FlipSprite(character.PlayerTransform);

        float distanceToPlayer = Vector2.Distance(character.transform.position, player.transform.position);

        if (distanceToPlayer > character.MonsterData.targetDistance)
        {
            stateManager.ChangeState(character.CreateMoveState());
            return;
        }

        if (character.CurrentAttackCooldown <= 0)
        {
            animator.SetTrigger("IsAttack");
            character.Attack(player);
            character.ResetAttackCooldown();
        }
    }
}
