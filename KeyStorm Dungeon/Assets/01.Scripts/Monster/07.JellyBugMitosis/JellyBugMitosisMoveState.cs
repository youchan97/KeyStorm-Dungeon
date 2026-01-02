using UnityEngine;

public class JellyBugMitosisMoveState : MonsterMoveState
{
    private JellyBugMitosis jellyBugMitosis;

    private float stoppedThreshold = 0.01f;
    private float checkInterval = 0.2f;
    private float timeSinceLastCheck;
    private Vector3 lastPosition;

    private float currentPoisonCooldown;
    public JellyBugMitosisMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        jellyBugMitosis = character as JellyBugMitosis;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;

        currentPoisonCooldown = 0f;
        lastPosition = jellyBugMitosis.transform.position;
        timeSinceLastCheck = 0f;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (rb == null) return;

        if (character.isKnockBack) return;

        rb.velocity = jellyBugMitosis.CurrentMoveDirection * character.MoveSpeed;

        jellyBugMitosis.FlipSprite(jellyBugMitosis.CurrentMoveDirection.x);

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

        currentPoisonCooldown -= Time.fixedDeltaTime;

        if (currentPoisonCooldown <= 0f)
        {
            jellyBugMitosis.SpawnPoisonField(jellyBugMitosis.transform.position + jellyBugMitosis.PoisonSpawnPointOffset);
            currentPoisonCooldown = jellyBugMitosis.PoisonCooldown;
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
