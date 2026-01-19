using UnityEngine;

public class WoodIdleState : MonsterIdleState
{
    private Wood wood;
    private float currentIdleTime;

    public WoodIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        wood = character as Wood;
    }

    public override void EnterState()
    {
        if (wood.MonsterRb != null)
        {
            wood.MonsterRb.velocity = Vector2.zero;
        }

        if (wood.PlayerGO == null)
        {
            return;
        }

        currentIdleTime = wood.IdleTime;
    }

    public override void UpdateState()
    {
        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(wood.CreateMoveState());
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
