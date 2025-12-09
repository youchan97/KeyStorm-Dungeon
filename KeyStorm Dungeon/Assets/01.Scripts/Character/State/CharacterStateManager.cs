using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager
{
    private CharacterState curState;
    private Character character;

    public CharacterStateManager(Character character)
    {
        this.character = character;
    }

    public void ChangeState(CharacterState newState)
    {
        if (curState == newState)
            return;

        if(curState != null)
            curState.ExitState();
        curState = newState;
        curState.EnterState();
    }

    public void Update()
    {
        if(curState != null && curState.UseFixedUpdate() == false)
        {
            curState.UpdateState();
        }
    }

    public void FixedUpdate()
    {
        if (curState != null && curState.UseFixedUpdate())
        {
            curState.FixedUpdateState();
        }
    }

    public void ResetState()
    {
        curState = null;
    }
}
