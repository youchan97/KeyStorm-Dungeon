using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : CharacterIdleState<Monster>
{
    public MonsterIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
