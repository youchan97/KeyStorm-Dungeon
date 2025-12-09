using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : CharacterIdleState<Player>
{
    public PlayerIdleState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }

    public override void UpdateState()
    {
    }
}
