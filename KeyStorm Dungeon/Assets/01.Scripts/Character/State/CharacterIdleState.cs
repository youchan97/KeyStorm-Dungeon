using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdleState<T> : CharacterState<T> where T : Character
{
    public CharacterIdleState(T character, CharacterStateManager<T> stateManager) : base(character, stateManager)
    {
    }
}
