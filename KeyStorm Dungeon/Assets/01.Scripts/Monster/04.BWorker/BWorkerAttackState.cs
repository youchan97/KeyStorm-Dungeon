using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWorkerAttackState : MonsterAttackState
{
    public BWorkerAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
