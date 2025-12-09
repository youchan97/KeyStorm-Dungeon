using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : CharacterMoveState<Monster>
{
    public MonsterMoveState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
