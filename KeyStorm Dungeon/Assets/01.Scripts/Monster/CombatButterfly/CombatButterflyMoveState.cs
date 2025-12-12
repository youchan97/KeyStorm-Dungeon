using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatButterflyMoveState : MonsterMoveState
{
    public CombatButterflyMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (character == null || rb == null) return;

        character.FlipSprite(playerTransform);

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        UpdateMovement(distanceToPlayer);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }
}
