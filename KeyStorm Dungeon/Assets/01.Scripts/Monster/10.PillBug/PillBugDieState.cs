using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBugDieState : MonsterDieState
{
    public PillBugDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }
}
