using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterAttackState : MonsterAttackState
{
    public MeleeMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
