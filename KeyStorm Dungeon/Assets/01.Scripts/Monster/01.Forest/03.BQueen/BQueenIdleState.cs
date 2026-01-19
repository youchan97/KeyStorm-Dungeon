using UnityEngine;
using static ConstValue;

public class BQueenIdleState : MonsterIdleState
{
    private BQueen bQueen;

    private float currentIdleTime;
    public BQueenIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        bQueen = character as BQueen;
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

        bQueen.Animator.SetFloat(AxisX, 0f);
        bQueen.Animator.SetFloat(AxisY, 0f);

        currentIdleTime = bQueen.IdleTime;
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        currentIdleTime -= Time.deltaTime;

        if (currentIdleTime <= 0)
        {
            stateManager.ChangeState(bQueen.CreateMoveState());
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
}
