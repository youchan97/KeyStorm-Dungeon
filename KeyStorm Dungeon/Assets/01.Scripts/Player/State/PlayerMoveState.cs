using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;
public class PlayerMoveState : CharacterMoveState<Player>
{
    Rigidbody2D rb;
    Vector2 vec;
    Animator anim;

    public PlayerMoveState(Player player, CharacterStateManager<Player> stateManager) : base(player, stateManager)
    {
    }

    public override bool UseFixedUpdate() => true;

    public override void EnterState()
    {
        rb = character.PlayerRb;
        anim = character.Anim;
        anim.SetBool(MoveAnim, true);
    }

    public override void FixedUpdateState()
    {
        if(!character.IsMove)
        {
            stateManager.ChangeState(character.IdleState);
            return;
        }
        if (character.IsDashing)
            return;
        vec = character.PlayerController.MoveVec;
        anim.SetFloat(AxisX, vec.x);
        anim.SetFloat(AxisY, vec.y);
        rb.velocity = vec.normalized * character.MoveSpeed;
    }

    public override void ExitState()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool(MoveAnim, false);
    }
}
