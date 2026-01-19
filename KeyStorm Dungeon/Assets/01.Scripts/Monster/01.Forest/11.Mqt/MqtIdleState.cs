using UnityEngine;

public class MqtIdleState : MonsterIdleState
{
    private Mqt mqt;
    private float currentIdleTime;

    public MqtIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        mqt = character as Mqt;
    }

    public override void EnterState()
    {
        if (mqt.MonsterRb != null)
        {
            mqt.MonsterRb.velocity = Vector2.zero;
        }

        if (mqt.PlayerGO == null)
        {
            return;
        }

        if (!mqt.AttackedPlayer)
        {
            currentIdleTime = mqt.IdleTime;
        }
        else
        {
            currentIdleTime = mqt.AttackedIdleTime;
        }
    }

    public override void UpdateState()
    {
        if (mqt.PlayerTransform == null || mqt.PlayerGO == null) return;

        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0)
        {
            if (!mqt.AttackedPlayer)
            {
                stateManager.ChangeState(mqt.CreateMoveState());
                return;
            }
            else
            {
                stateManager.ChangeState(mqt.CreateAttackState());
                return;
            }
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
