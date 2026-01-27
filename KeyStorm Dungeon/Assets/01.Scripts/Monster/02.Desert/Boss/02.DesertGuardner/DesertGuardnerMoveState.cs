using UnityEngine;
using static ConstValue;

public class DesertGuardnerMoveState : MonsterMoveState
{
    private DesertGuardner desertGuardner;
    private float currentMoveTime;

    public DesertGuardnerMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        desertGuardner = character as DesertGuardner;
    }

    public override void EnterState()
    {
        base.EnterState();
        currentMoveTime = desertGuardner.MoveTime;
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        currentMoveTime -= Time.deltaTime;

        if(currentMoveTime <= 0)
        {
            stateManager.ChangeState(desertGuardner.CreateAttackState());
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }
}
