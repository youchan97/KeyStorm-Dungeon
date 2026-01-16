using UnityEngine;

public class MqtMoveState : MonsterMoveState
{
    private Mqt mqt;
    private Vector2 directionToPlayer;
    private float currentMoveTime;

    public MqtMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        mqt = character as Mqt;
    }

    public override void EnterState()
    {
        rb = mqt.MonsterRb;
        animator = mqt.Animator;
        playerTransform = mqt.PlayerTransform;

        directionToPlayer = (mqt.PlayerTransform.position - mqt.transform.position).normalized;
        currentMoveTime = mqt.MoveTime;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (mqt.PlayerGO == null)
        {
            mqt.ChangeStateToPlayerDied();
            return;
        }

        if (character.isKnockBack) return;

        currentMoveTime -= Time.deltaTime;

        if (currentMoveTime <= 0)
        {
            stateManager.ChangeState(mqt.CreateIdleState());
            return;
        }

        rb.velocity = directionToPlayer * mqt.MoveSpeed;

        if (mqt.AttackedPlayer)
        {
            stateManager.ChangeState(mqt.CreateAttackState());
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
