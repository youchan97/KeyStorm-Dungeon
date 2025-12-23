using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeFlowerIdleState : MonsterIdleState
{
    private SporeFlower sporeFlower;
    private float currentIdleTime;

    public SporeFlowerIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.sporeFlower = character as SporeFlower;
    }

    public override void EnterState()
    {
        base.EnterState();
        currentIdleTime = sporeFlower.IdleTime;
    }

    public override void UpdateState()
    {
        if (character.PlayerTransform == null || character.PlayerGO == null) return;

        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0f)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void ExitState()
    {
        base.ExitState();
        currentIdleTime = sporeFlower.IdleTime;
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
