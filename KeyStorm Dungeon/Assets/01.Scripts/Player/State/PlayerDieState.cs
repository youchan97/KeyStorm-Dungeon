using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class PlayerDieState : CharacterDieState<Player>
{
    Animator anim;
    public PlayerDieState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }

    public override void EnterState()
    {
        anim = character.Anim;
        anim.SetTrigger(DieAnim);
        character.PlayerController.DisableInput();
    }


}
