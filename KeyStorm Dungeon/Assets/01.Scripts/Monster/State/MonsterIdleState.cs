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
    }

    public override void FixedUpdateState()
    {
        if (character.playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.playerTransform.position);

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
