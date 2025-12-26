using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWorkerIdleState : MonsterIdleState
{
    private BWorker bWorker;

    private Vector2 currentIdleMoveDirection;
    private float currentIdleMoveTime;
    private float idleMoveTime = 1f;
    private float idleMoveSpeedMultiplier = 0.2f;

    private float bQueenRange = 2f;

    public BWorkerIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.bWorker = character as BWorker;
    }

    public override void EnterState()
    {
        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        if (character.PlayerGO == null)
        {
            return;
        }

        SetNewRandomMoveDirection();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (bWorker.IsSpawnedImpulseActive)
        {
            bWorker.DecrementSpawnImpulseTime(Time.fixedDeltaTime);

            if (!bWorker.IsSpawnedImpulseActive)
            {
                bWorker.MonsterRb.velocity = Vector2.zero;
            }
            return;
        }

        if(bWorker.AssignedBQueen == null || bWorker.AssignedBQueen.isDamaged == true)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }
        else
        {
            float distanceToBQueen = Vector2.Distance(bWorker.transform.position, bWorker.AssignedBQueen.transform.position);
            if (distanceToBQueen > bQueenRange)
            {
                stateManager.ChangeState(bWorker.CreateMoveState());
                return;
            }
        }

        currentIdleMoveTime -= Time.fixedDeltaTime;

        if(currentIdleMoveTime <= 0)
        {
            SetNewRandomMoveDirection();
        }

        bWorker.MonsterRb.velocity = currentIdleMoveDirection * (bWorker.MoveSpeed * idleMoveSpeedMultiplier);
        bWorker.FlipSprite(-currentIdleMoveDirection.x);

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private void SetNewRandomMoveDirection()
    {
        currentIdleMoveDirection = Random.insideUnitCircle.normalized;
        currentIdleMoveTime = idleMoveTime;
    }
}
