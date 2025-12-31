using UnityEngine;
using static ConstValue;

public class PollenButterflyIdleState : MonsterIdleState
{
    private PollenButterfly pollenButterfly;

    private Animator animator;
    private Vector2 currentIdleMoveDirection;
    private float currentIdleMoveTime;
    private float idleMoveTime;
    private float idleRange;
    private float idleMoveSpeedMultiplier;

    public PollenButterflyIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        pollenButterfly = character as PollenButterfly;
    }

    public override void EnterState()
    {
        animator = character.Animator;

        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
        }

        if (character.PlayerGO == null)
        {
            return;
        }

        idleMoveTime = pollenButterfly.IdleMoveTime;
        idleRange = pollenButterfly.IdleRange;
        idleMoveSpeedMultiplier = pollenButterfly.IdleMoveSpeedMultiplier;

        SetNewRandomMoveDirection();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (character.isKnockBack) return;

        float distanceToSpawnPoint = Vector2.Distance(pollenButterfly.transform.position, pollenButterfly.SpawnPosition);
        if (distanceToSpawnPoint > idleRange)
        {
            stateManager.ChangeState(pollenButterfly.CreateMoveState());
            return;
        }

        currentIdleMoveTime -= Time.fixedDeltaTime;

        if(currentIdleMoveTime <= 0)
        {
            SetNewRandomMoveDirection();
        }

        pollenButterfly.MonsterRb.velocity = currentIdleMoveDirection * (pollenButterfly.MoveSpeed * idleMoveSpeedMultiplier);
        UpdateAnimation();

        float distanceToPlayer = Vector2.Distance(character.transform.position, character.PlayerTransform.position);

        if (distanceToPlayer <= character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateAttackState());
            return;
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

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetFloat(AxisX, currentIdleMoveDirection.x);
        animator.SetFloat(AxisY, currentIdleMoveDirection.y);
    }

    private void SetNewRandomMoveDirection()
    {
        currentIdleMoveDirection = Random.insideUnitCircle.normalized;
        currentIdleMoveTime = idleMoveTime;
    }
}
