using UnityEngine;
using static ConstValue;

public class WoodsRootAttackState : MonsterAttackState
{
    private WoodsRoot woodsRoot;

    public WoodsRootAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        woodsRoot = character as WoodsRoot;
    }

    public override void EnterState()
    {
        animator = woodsRoot.Animator;
    }

    public override void UpdateState()
    {
        if (woodsRoot.PlayerGO == null)
        {
            woodsRoot.ChangeStateToPlayerDied();
            return;
        }

        animator.SetTrigger(AttackAnim);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
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
