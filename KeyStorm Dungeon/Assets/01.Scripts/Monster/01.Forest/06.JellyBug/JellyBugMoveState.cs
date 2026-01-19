using UnityEngine;

public class JellyBugMoveState : MonsterMoveState
{
    private JellyBug jellyBug;

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    public JellyBugMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        jellyBug = character as JellyBug;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;

        lastPosition = jellyBug.transform.position;
        timeSinceLastCheck = 0f;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (rb == null) return;

        if (character.isKnockBack) return;

        rb.velocity = jellyBug.CurrentMoveDirection * character.MoveSpeed;

        jellyBug.FlipSprite(jellyBug.CurrentMoveDirection.x);

        timeSinceLastCheck += Time.fixedDeltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            float distanceMoved = Vector3.Distance(jellyBug.transform.position, lastPosition);

            if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude < stoppedThreshold)
            {
                rb.velocity = Vector2.zero;
                stateManager.ChangeState(jellyBug.CreateIdleState());
                return;
            }

            lastPosition = jellyBug.transform.position;
            timeSinceLastCheck = 0f;
        }
    }

    public override void ExitState()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }
}
