using UnityEngine;

public class DesertGuardnerIdleState : MonsterIdleState
{
    private DesertGuardner desertGuardner;
    private float currentIdleTime;

    public DesertGuardnerIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        desertGuardner = character as DesertGuardner;
    }

    public override void EnterState()
    {
        base.EnterState();
        currentIdleTime = desertGuardner.IdleTime;
    }

    public override void UpdateState()
    {
        if (desertGuardner.PlayerGO == null) return;

        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(desertGuardner.CreateMoveState());
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
