using UnityEngine;
using static ConstValue;

public class CombatButterflyIdleState : MonsterIdleState
{
    public CombatButterflyIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        if (character.Animator != null)
        {
            character.Animator.SetBool(MoveAnim, false);
        }
        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        if (character.player.Hp <= 0)
        {
            return;
        }

        stateManager.ChangeState(character.CreateMoveState());
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
