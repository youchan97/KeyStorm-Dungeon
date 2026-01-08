using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : MonsterAttackState
{
    public SlimeAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
