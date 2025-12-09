using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : CharacterAttackState<Monster>
{
    public MonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
