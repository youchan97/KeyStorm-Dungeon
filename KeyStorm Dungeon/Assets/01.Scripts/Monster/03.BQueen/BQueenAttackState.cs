using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BQueenAttackState : MonsterAttackState
{
    public BQueenAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }
}
