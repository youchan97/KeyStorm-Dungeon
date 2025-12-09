using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackState<T> : CharacterState<T> where T : Character
{
    public CharacterAttackState(T character, CharacterStateManager<T> stateManager) : base(character, stateManager)
    {
    }
}
