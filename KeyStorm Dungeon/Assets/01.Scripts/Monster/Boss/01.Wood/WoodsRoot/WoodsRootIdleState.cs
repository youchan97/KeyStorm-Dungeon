using UnityEngine;

public class WoodsRootIdleState : MonsterIdleState
{
    private WoodsRoot woodsRoot;

    private const string IdleAnim = "IsIdle";

    public WoodsRootIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        woodsRoot = character as WoodsRoot;
    }

    public override void EnterState()
    {
        if (woodsRoot.PlayerGO == null)
        {
            return;
        }

        woodsRoot.Animator.SetBool(IdleAnim, true);
    }

    public override void UpdateState()
    {
        if ( woodsRoot.CurrentSlashAttackCooldown <= 0)
        {
            float distanceToPlayer = Vector2.Distance(woodsRoot.transform.position, woodsRoot.PlayerTransform.position);

            if (distanceToPlayer <= woodsRoot.MonsterData.detectRange)
            {
                stateManager.ChangeState(woodsRoot.CreateAttackState());
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
        woodsRoot.Animator.SetBool(IdleAnim, false);
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }
}
