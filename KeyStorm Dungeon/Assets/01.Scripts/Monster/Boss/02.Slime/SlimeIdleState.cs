using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIdleState : MonsterIdleState
{
    public SlimeIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
