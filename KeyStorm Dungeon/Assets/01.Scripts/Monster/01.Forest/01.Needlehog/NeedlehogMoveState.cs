using UnityEngine;
using static ConstValue;

public class NeedlehogMoveState : MonsterMoveState
{
    private Needlehog needlehog;
    private float currentMoveTime;
    private Vector2 currentRandomDirection;
    private Vector2[] cardinalDirections = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    public NeedlehogMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        needlehog = character as Needlehog;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        animator.SetBool(MoveAnim, true);
        playerTransform = character.PlayerTransform;

        currentRandomDirection = GetRandomDirection();

        currentMoveTime = needlehog.MoveTime;
        UpdateAnimation();

        lastPosition = needlehog.transform.position;
        timeSinceLastCheck = 0f;

        needlehog.OnWallOrCollisionHit += OnCollisionDetected;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (character.isKnockBack) return;

        if (currentMoveTime > 0f)
        {
            rb.velocity = currentRandomDirection * needlehog.MoveSpeed;
            currentMoveTime -= Time.fixedDeltaTime;

            UpdateAnimation();

            timeSinceLastCheck += Time.fixedDeltaTime;
            if (timeSinceLastCheck >= checkInterval)
            {
                float distanceMoved = Vector3.Distance(needlehog.transform.position, lastPosition);

                if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude < stoppedThreshold)
                {
                    rb.velocity = Vector2.zero;
                    stateManager.ChangeState(needlehog.CreateIdleState());
                    return;
                }

                lastPosition = needlehog.transform.position;
                timeSinceLastCheck = 0f;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            stateManager.ChangeState(needlehog.CreateIdleState());
            return;
        }

        if (needlehog.PlayerTransform != null && needlehog.PlayerGO != null)
        {
            float distanceToPlayer = Vector2.Distance(needlehog.transform.position, needlehog.PlayerTransform.position);
            if (distanceToPlayer <= needlehog.MonsterData.detectRange)
            {
                rb.velocity = Vector2.zero;
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

        needlehog.OnWallOrCollisionHit -= OnCollisionDetected;
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetFloat(AxisX, currentRandomDirection.x);
        animator.SetFloat(AxisY, currentRandomDirection.y);
    }

    private Vector2 GetRandomDirection()
    {
        return cardinalDirections[Random.Range(0, cardinalDirections.Length)];
    }

    private void OnCollisionDetected(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        stateManager.ChangeState(needlehog.CreateIdleState());
    }
}
