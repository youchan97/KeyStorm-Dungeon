using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    protected Character character;
    protected CharacterStateManager stateManager;

    public CharacterState(Character character, CharacterStateManager stateManager)
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
