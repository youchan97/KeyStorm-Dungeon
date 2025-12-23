using UnityEngine;
using static ConstValue;

public class CombatButterflyMoveState : MonsterMoveState
{
    public CombatButterflyMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (character == null || rb == null) return;

        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        character.FlipSprite(rb.velocity.x);

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        UpdateAnimation();

        if (Time.time >= nextPathUpdateTime)
        {
            RequestNewPath();
            nextPathUpdateTime = Time.time + pathUpdateInterval;
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            UpdateMovement();
        }
        else
        {
            //rb.velocity = Vector2.zero;
            UpdateDirectionMovement(distanceToPlayer);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }

    public void UpdateAnimation()
    {
        if (character.Animator == null || playerTransform == null) return;
        Vector2 monsterPos = character.transform.position;
        Vector2 playerPos = playerTransform.position;

        Vector2 desiredMoveDirection = (playerPos - monsterPos).normalized;

        float moveX = desiredMoveDirection.x;
        float moveY = desiredMoveDirection.y;

        character.Animator.SetFloat(AxisX, moveX);
        character.Animator.SetFloat(AxisY, moveY);
    }
}
