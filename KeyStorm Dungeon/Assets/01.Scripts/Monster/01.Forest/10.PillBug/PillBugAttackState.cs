using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBugAttackState : MonsterAttackState
{
    public PillBugAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
