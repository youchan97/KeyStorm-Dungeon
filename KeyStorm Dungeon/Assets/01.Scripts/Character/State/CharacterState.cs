using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState<T> where T : Character
{
    protected T character;
    protected CharacterStateManager<T> stateManager;

    public CharacterState(T character, CharacterStateManager<T> stateManager)
    {
        this.character = character;
        this.stateManager = stateManager;
    }

    public virtual bool UseFixedUpdate() => false;
    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual void ExitState() { }
}
