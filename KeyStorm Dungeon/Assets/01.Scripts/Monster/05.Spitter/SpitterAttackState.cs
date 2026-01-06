using UnityEngine;
using static ConstValue;

public class SpitterAttackState : MonsterAttackState
{
    public SpitterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
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
