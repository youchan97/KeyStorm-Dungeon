using UnityEngine;
using static ConstValue;
public class MonsterIdleState : CharacterIdleState<Monster>
{
    public MonsterIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
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

        if (character.MonsterData.tier == MonsterTier.Boss)
        {
            stateManager.ChangeState(character.CreateMoveState());
            return;
        }
    }

    public override void FixedUpdateState()
    {
        if (character.PlayerTransform == null || character.player.Hp <= 0) return;

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateMoveState());
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
