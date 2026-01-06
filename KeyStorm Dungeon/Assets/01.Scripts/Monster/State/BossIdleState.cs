using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : MonsterIdleState
{
    public BossIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (character.player != null)
        {
            stateManager.ChangeState(character.CreateMoveState());
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
