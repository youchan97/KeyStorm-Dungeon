using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : CharacterDieState<Player>
{
    public PlayerDieState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }

}
