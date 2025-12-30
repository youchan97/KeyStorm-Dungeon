using UnityEngine;

public class BWorkerMoveState : MonsterMoveState
{
    private BWorker bWorker;
    private float arriveThreshold = 1f;

    public BWorkerMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        bWorker = character as BWorker;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = bWorker.Animator;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (bWorker.PlayerGO == null)
        {
            bWorker.ChangeStateToPlayerDied();
            return;
        }

        if (bWorker.AssignedBQueen == null || bWorker.AssignedBQueen.isDamaged)
        {
            stateManager.ChangeState(bWorker.CreateAttackState());
            return;
        }

        if (character.isKnockBack) return;

        Vector2 bQueenPosition = bWorker.AssignedBQueen.transform.position;
        Vector2 currentPosition = bWorker.transform.position;

        if (Vector2.Distance(currentPosition, bQueenPosition) < arriveThreshold)
        {
            stateManager.ChangeState(bWorker.CreateIdleState());
            return;
        }

        Vector2 moveDirection = (bQueenPosition - currentPosition).normalized;
        rb.velocity = moveDirection * character.MoveSpeed;

        bWorker.FlipSprite(-moveDirection.x);

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
        }
    }

    public override void ExitState()
    {
        if(rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
