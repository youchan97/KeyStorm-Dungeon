using UnityEngine;

public class MqtAttackState : MonsterAttackState
{
    private Mqt mqt;
    private Vector2 directionToPlayer;
    private float currentAttackMoveTime;
    public MqtAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        mqt = character as Mqt;
    }

    public override void EnterState()
    {
        base.EnterState();

        directionToPlayer = (mqt.PlayerTransform.position - mqt.transform.position).normalized;
        currentAttackMoveTime = mqt.AttackedMoveTime;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (mqt.PlayerTransform == null) return;

        if (mqt.PlayerGO == null)
        {
            mqt.ChangeStateToPlayerDied();
            return;
        }

        if (character.isKnockBack) return;

        currentAttackMoveTime -= Time.deltaTime;

        if (currentAttackMoveTime <= 0)
        {
            stateManager.ChangeState(mqt.CreateIdleState());
            return;
        }

        rb.velocity = directionToPlayer * (mqt.MoveSpeed * mqt.AttackedMoveSpeedMultiple);
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
