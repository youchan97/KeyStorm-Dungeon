using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodDieState : MonsterDieState
{
    public WoodDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }
}
