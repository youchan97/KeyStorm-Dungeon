using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterDieState : MonsterDieState
{
    public SpitterDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }
}
