using UnityEngine;

public class JellyBugMitosisMoveState : MonsterMoveState
{
    private JellyBugMitosis jellyBugMitosis;

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    private LayerMask otherMonsterLayer;
    private float otherMonsterDetectionDistance;
    private CapsuleCollider2D currentMonsterCollider;
    private float poisonCooldown;
    public JellyBugMitosisMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        jellyBugMitosis = character as JellyBugMitosis;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;

        otherMonsterLayer = jellyBugMitosis.OtherMonsterLayer;
        otherMonsterDetectionDistance = jellyBugMitosis.OtherMonsterDetectionDistance;
        currentMonsterCollider = jellyBugMitosis.GetCurrentMonsterCollider();
        poisonCooldown = 0f;

        lastPosition = jellyBugMitosis.transform.position;
        timeSinceLastCheck = 0f;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (rb == null) return;

        rb.velocity = jellyBugMitosis.CurrentMoveDirection * character.MoveSpeed;

        jellyBugMitosis.FlipSprite(jellyBugMitosis.CurrentMoveDirection.x);

        if (currentMonsterCollider != null && otherMonsterLayer != 0)
        {
            Vector2 detectionCenter = (Vector2)jellyBugMitosis.transform.position
                + (jellyBugMitosis.CurrentMoveDirection.normalized * (currentMonsterCollider.bounds.extents.magnitude + otherMonsterDetectionDistance / 2));

            Vector2 detectionSize = new Vector2(currentMonsterCollider.size.x, currentMonsterCollider.size.y);

            CapsuleDirection2D capsuleDir = currentMonsterCollider.direction;

            Collider2D[] hits = Physics2D.OverlapCapsuleAll(detectionCenter, detectionSize, CapsuleDirection2D.Horizontal, 0f, otherMonsterLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != jellyBugMitosis.gameObject)
                {
                    rb.velocity = Vector2.zero;
                    stateManager.ChangeState(jellyBugMitosis.CreateIdleState());
                    return;
                }
            }
        }
        timeSinceLastCheck += Time.fixedDeltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            float distanceMoved = Vector3.Distance(jellyBugMitosis.transform.position, lastPosition);

            if (distanceMoved < stoppedThreshold && rb.velocity.sqrMagnitude > stoppedThreshold * stoppedThreshold)
            {
                rb.velocity = Vector2.zero;
                stateManager.ChangeState(jellyBugMitosis.CreateIdleState());
                return;
            }

            lastPosition = jellyBugMitosis.transform.position;
            timeSinceLastCheck = 0f;
        }

        poisonCooldown -= Time.fixedDeltaTime;

        if (poisonCooldown <= 0f)
        {
            jellyBugMitosis.SpawnPoisonField(jellyBugMitosis.transform.position + new Vector3(0, -0.3f, 0));
            poisonCooldown = jellyBugMitosis.PoisonCooldown;
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
