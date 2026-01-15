using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MqtDieState : MonsterDieState
{
    public MqtDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }
}
