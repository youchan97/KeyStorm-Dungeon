using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDieState : MonsterDieState
{
    public SlimeDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }
}
