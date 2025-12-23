using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedlehogIdleState : MonsterIdleState
{
    private Needlehog needlehog;
    private float currentIdleTime;

    public NeedlehogIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.needlehog = character as Needlehog;
    }

    public override void EnterState()
    {
        base.EnterState();
        currentIdleTime = needlehog.IdleTime;
    }

    public override void UpdateState()
    {
        if (character.PlayerTransform == null || character.PlayerGO == null) return;

        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0f)
        {
            stateManager.ChangeState(character.CreateMoveState());
            return;
        }

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
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
        currentIdleTime = needlehog.IdleTime;
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
