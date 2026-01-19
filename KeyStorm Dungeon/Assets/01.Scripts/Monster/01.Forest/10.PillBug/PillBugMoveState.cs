using UnityEngine;
using static ConstValue;

public class PillBugMoveState : MonsterMoveState
{
    private PillBug pillBug;

    #region 애니메이션
    private const string RollAnim = "Roll";
    #endregion

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;
    private Coroutine coroutine;

    public PillBugMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        pillBug = character as PillBug;
    }

    public override void EnterState()
    {
        rb = pillBug.MonsterRb;
        animator = pillBug.Animator;
        playerTransform = pillBug.PlayerTransform;

        lastPosition = pillBug.transform.position;
        timeSinceLastCheck = 0f;

        animator.SetBool(RollAnim, true);
        pillBug.ChangeIsChase(true);
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (pillBug.PlayerGO == null)
        {
            pillBug.ChangeStateToPlayerDied();
            return;
        }

        if (pillBug.isKnockBack || pillBug.IsReaction) return;

        rb.velocity = pillBug.CurrentMoveDirection * pillBug.MoveSpeed * pillBug.MoveSpeedMultiple;

        timeSinceLastCheck += Time.fixedDeltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            float distanceMoved = Vector3.Distance(pillBug.transform.position, lastPosition);

            if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude < stoppedThreshold)
            {
                Vector2 reactionDirection = -pillBug.CurrentMoveDirection.normalized;
                coroutine = pillBug.StartCoroutine(pillBug.ObstacleReaction(reactionDirection));
                return;
            }

            lastPosition = pillBug.transform.position;
            timeSinceLastCheck = 0f;
        }

        animator.SetFloat(AxisX, rb.velocity.x);
        animator.SetFloat(AxisY, rb.velocity.y);
    }

    public override void ExitState()
    {
        pillBug.ChangeIsChase(false);
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }
}
