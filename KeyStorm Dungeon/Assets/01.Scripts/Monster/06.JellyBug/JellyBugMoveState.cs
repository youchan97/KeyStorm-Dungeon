using UnityEngine;

public class JellyBugMoveState : MonsterMoveState
{
    private JellyBug jellyBug;

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    private LayerMask otherMonsterLayer;
    private float otherMonsterDetectionDistance;
    private CapsuleCollider2D currentMonsterCollider;

    public JellyBugMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        jellyBug = character as JellyBug;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;

        otherMonsterLayer = jellyBug.OtherMonsterLayer;
        otherMonsterDetectionDistance = jellyBug.OtherMonsterDetectionDistance;
        currentMonsterCollider = jellyBug.GetCurrentMonsterCollider();

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

        if (currentMonsterCollider != null && otherMonsterLayer != 0)
        {
            Vector2 detectionCenter = (Vector2)jellyBug.transform.position
                + (jellyBug.CurrentMoveDirection.normalized * (currentMonsterCollider.bounds.extents.magnitude + otherMonsterDetectionDistance / 2));

            Vector2 detectionSize = new Vector2(currentMonsterCollider.size.x, currentMonsterCollider.size.y);

            CapsuleDirection2D capsuleDir = currentMonsterCollider.direction;

            Collider2D[] hits = Physics2D.OverlapCapsuleAll(detectionCenter, detectionSize, CapsuleDirection2D.Horizontal, 0f, otherMonsterLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != jellyBug.gameObject)
                {
                    rb.velocity = Vector2.zero;
                    stateManager.ChangeState(jellyBug.CreateIdleState());
                    return;
                }
            }
        }
        timeSinceLastCheck += Time.fixedDeltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            float distanceMoved = Vector3.Distance(jellyBug.transform.position, lastPosition);

            if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude > stoppedThreshold * stoppedThreshold)
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
