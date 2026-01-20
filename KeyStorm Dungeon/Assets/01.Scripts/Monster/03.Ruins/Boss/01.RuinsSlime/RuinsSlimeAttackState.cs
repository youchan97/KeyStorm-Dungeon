using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class RuinsSlimeAttackState : SlimeAttackState
{
    public RuinsSlimeAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
