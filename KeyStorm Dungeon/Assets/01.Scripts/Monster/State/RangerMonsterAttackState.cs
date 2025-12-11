using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMonsterAttackState : MonsterAttackState
{
    private Player player;
    private Animator animator;

    public RangerMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        animator = character.Animator;

        if (character.playerGO != null)
        {
            player = character.playerGO.GetComponent<Player>();
            character.SetAttackTarget(player);
            if (player == null)
            {
                Debug.LogError("MonsterAttackState: Player GameObject에 Player컴포넌트가 없음");
            }
        }
        else
        {
            Debug.LogError("MonsterAttackState: Monster.playerGO가 할당되지 않음");
        }

        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }
    }

    public override void UpdateState()
    {
        if (player == null)
        {
            stateManager.ChangeState(character.CreateIdleState());
            return;
        }

        character.FlipSprite(character.playerTransform);

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

    public override void ExitState()
    {
        if (animator != null)
        {
            animator.ResetTrigger("IsAttack");
        }
        character.SetAttackTarget(null);
    }
}
