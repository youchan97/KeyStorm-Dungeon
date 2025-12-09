using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : CharacterDieState<Monster>
{
    public MonsterDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
