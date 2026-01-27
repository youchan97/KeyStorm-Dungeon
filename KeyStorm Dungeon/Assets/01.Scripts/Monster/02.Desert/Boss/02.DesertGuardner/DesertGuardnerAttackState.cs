using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertGuardnerAttackState : MonsterAttackState
{
    private DesertGuardner desertGuardner;

    public DesertGuardnerAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        desertGuardner = character as DesertGuardner;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
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
