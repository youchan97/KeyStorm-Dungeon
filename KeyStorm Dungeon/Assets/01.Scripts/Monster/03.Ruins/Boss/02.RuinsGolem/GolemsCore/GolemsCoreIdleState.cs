using UnityEngine;

public class GolemsCoreIdleState : MonsterIdleState
{
    private GolemsCore golemsCore;
    private float currentIdleTime;

    public GolemsCoreIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        golemsCore = character as GolemsCore;
    }

    public override void EnterState()
    {
        if (golemsCore.MonsterRb != null)
        {
            golemsCore.MonsterRb.velocity = Vector2.zero;
        }

        currentIdleTime = golemsCore.IdleTime;
    }

    public override void UpdateState()
    {
        currentIdleTime -= Time.deltaTime;
        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(golemsCore.CreateMoveState());
            return;
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
