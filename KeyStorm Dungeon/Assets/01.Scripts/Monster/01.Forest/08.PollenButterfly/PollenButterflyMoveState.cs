using UnityEngine;
using static ConstValue;

public class PollenButterflyMoveState : MonsterMoveState
{
    private PollenButterfly pollenButterfly;
    private float arriveThreshold = 1f;

    public PollenButterflyMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        pollenButterfly = character as PollenButterfly;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = pollenButterfly.Animator;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (pollenButterfly.PlayerGO == null)
        {
            pollenButterfly.ChangeStateToPlayerDied();
            return;
        }

        if (character.isKnockBack) return;

        Vector2 spawnPoint = pollenButterfly.SpawnPosition;
        Vector2 currentPosition = pollenButterfly.transform.position;

        if (Vector2.Distance(currentPosition, spawnPoint) < arriveThreshold)
        {
            stateManager.ChangeState(pollenButterfly.CreateIdleState());
            return;
        }

        Vector2 moveDirction = (spawnPoint - currentPosition).normalized;
        rb.velocity = moveDirction * character.MoveSpeed;

        animator.SetFloat(AxisX, moveDirction.x);
        animator.SetFloat(AxisY, moveDirction.y);

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
