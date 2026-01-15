using UnityEngine;
using static ConstValue;

public class SpitterAttackState : MonsterAttackState
{
    public SpitterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        animator = character.Animator;

        if (character.PlayerGO != null)
        {
            player = character.PlayerGO.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("MonsterAttackState: Player GameObject에 Player컴포넌트가 없음");
            }
        }
        else
        {
            Debug.LogError("MonsterAttackState: Monster.playerGO가 할당되지 않음");
        }
    }

    public override void UpdateState()
    {
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        character.FlipSpriteAttack(player.transform);

        float distanceToPlayer = Vector2.Distance(character.transform.position, player.transform.position);

        if (distanceToPlayer > character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateIdleState());
            return;
        }

        if (character.CurrentAttackCooldown <= 0)
        {
            animator.SetTrigger(AttackAnim);
            character.ResetAttackCooldown();
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
