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
        // 임시로 플레이어의 사망을 체크
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        character.FlipSpriteAttack(player.transform);

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
