using UnityEngine;
using static ConstValue;

public class NeedlehogMoveState : MonsterMoveState
{
    private Needlehog needlehog;
    private float currentMoveTime;
    private Vector2 currentRandomDirection;

    private Vector2[] cardinalDirections = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

    public NeedlehogMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        this.needlehog = character as Needlehog;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        animator.SetBool(MoveAnim, true);
        playerTransform = character.PlayerTransform;

        currentRandomDirection = GetRandomDirection();

        currentMoveTime = needlehog.MoveTime;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (currentMoveTime > 0f)
        {
            rb.velocity = currentRandomDirection * needlehog.MoveSpeed;
            currentMoveTime -= Time.fixedDeltaTime;

            needlehog.UpdateAnimation();
        }
        else
        {
            stateManager.ChangeState(needlehog.CreateIdleState());
            return;
        }

        if (needlehog.PlayerTransform != null && needlehog.player.Hp > 0)
        {
            float distanceToPlayer = Vector2.Distance(needlehog.transform.position, needlehog.PlayerTransform.position);
            if (distanceToPlayer <= needlehog.MonsterData.detectRange)
            {
                stateManager.ChangeState(needlehog.CreateAttackState());
                return;
            }
        }
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool(MoveAnim, false);
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private void UpdateAnimation()
    {
        if (character.Animator == null) return;

    }

    private Vector2 GetRandomDirection()
    {
        return cardinalDirections[Random.Range(0, cardinalDirections.Length)];
    }
}
