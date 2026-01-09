using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIdleState : MonsterIdleState
{
    private Slime slime;
    private float currentIdleTime;

    public SlimeIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        slime = character as Slime;
    }

    public override void EnterState()
    {
        if (slime.MonsterRb != null)
        {
            slime.MonsterRb.velocity = Vector2.zero;
        }

        if (slime.PlayerGO == null) return;

        currentIdleTime = slime.IdleTime;
    }

    public override void UpdateState()
    {
        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(slime.CreateAttackState());
            return;
        }
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
