using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveState<T> : CharacterState<T> where T : Character
{
    public CharacterMoveState(T character, CharacterStateManager<T> stateManager) : base(character, stateManager)
    {
    }

}
