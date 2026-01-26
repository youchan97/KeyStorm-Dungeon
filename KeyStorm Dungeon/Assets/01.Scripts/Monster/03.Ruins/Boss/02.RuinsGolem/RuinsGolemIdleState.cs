using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinsGolemIdleState : MonsterIdleState
{
    private RuinsGolem ruinsGolem;
    private float currentIdleTime;

    public RuinsGolemIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        ruinsGolem = character as RuinsGolem;
    }

    public override void EnterState()
    {
        currentIdleTime = ruinsGolem.IdleTime;
    }

    public override void UpdateState()
    {
        if (ruinsGolem.PlayerGO == null) return;

        currentIdleTime -= Time.deltaTime;
        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(ruinsGolem.CreateAttackState());
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
