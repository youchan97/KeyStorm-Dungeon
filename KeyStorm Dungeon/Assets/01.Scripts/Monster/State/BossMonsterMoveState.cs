using UnityEngine;
using static ConstValue;

public class BossMonsterMoveState : MonsterMoveState
{
    private BossMonster boss;

    public BossMonsterMoveState(BossMonster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        boss = character as BossMonster;
    }

    public override void EnterState()
    {
        base.EnterState();
        boss.ResetPatternCooldown();
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null || boss == null || rb == null) return;

        boss.FlipSprite(rb.velocity.x);

        float distanceToPlayer = Vector2.Distance(boss.transform.position, playerTransform.position);

        if (boss.CurrentPatternCooldown <= 0)
        {
            stateManager.ChangeState(boss.CreateAttackState());
            return;
        }

        UpdateDirectionMovement(distanceToPlayer);
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
