using UnityEngine;

public class SpitterIdleState : MonsterIdleState
{

    public SpitterIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        if (character.PlayerGO == null)
        {
            return;
        }
    }

    public override void UpdateState()
    {
        if (character.PlayerTransform == null || character.PlayerGO == null) return;

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
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
