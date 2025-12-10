using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class MonsterAttackState : CharacterAttackState<Monster>
{
    private Player player;
    private Animator animator;

    public MonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        animator = character.Animator;
        
        if(character.playerGO != null)
        {
            player = character.playerGO.GetComponent<Player>();
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
            stateManager.ChangeState(character.IdleState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(character.transform.position, player.transform.position);

        if (character is RangerMonster)
        {
            if (distanceToPlayer > character.MonsterData.attackRange)
            {
                stateManager.ChangeState(character.MoveState);
                return;
            }
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
    }
}
