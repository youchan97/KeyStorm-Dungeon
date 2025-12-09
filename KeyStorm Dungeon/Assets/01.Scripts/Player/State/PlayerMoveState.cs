using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : CharacterMoveState<Player>
{
    public PlayerMoveState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }
}
