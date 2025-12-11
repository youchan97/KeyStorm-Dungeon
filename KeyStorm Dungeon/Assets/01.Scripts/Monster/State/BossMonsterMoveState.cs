using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class BossMonsterMoveState : MonsterMoveState
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    public BossMonsterMoveState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        character.ResetAttackCooldown();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null || character == null || rb == null) return;

        character.FlipSprite(character.playerTransform);

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        if (character.CurrentAttackCooldown <= 0)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }

        UpdateMovement(distanceToPlayer);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
