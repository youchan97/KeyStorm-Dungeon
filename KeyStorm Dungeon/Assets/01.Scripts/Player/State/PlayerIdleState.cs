using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : CharacterIdleState<Player>
{
    public PlayerIdleState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }

    public override void EnterState()
    {
        Debug.Log("안녕");
    }

    public override void UpdateState()
    {
        if (character.IsMove)
        {
            stateManager.ChangeState(character.MoveState);
            return;
        }
    }
}
