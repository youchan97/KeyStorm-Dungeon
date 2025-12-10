using UnityEngine;
using static ConstValue;

public class MonsterMoveState : CharacterMoveState<Monster>
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    public MonsterMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        animator.SetBool(MoveAnim, true);
        playerTransform = character.playerTransform;
    }

    public override void UpdateState()
    {
        if (playerTransform == null) return;

        Vector2 directionToPlayer = (playerTransform.position - character.transform.position).normalized;
        if (directionToPlayer.x < 0)
        {
            character.transform.localScale = new Vector3(Mathf.Abs(character.transform.localScale.x) * -1, character.transform.localScale.y, character.transform.localScale.z);
        }
        else if (directionToPlayer.x > 0)
        {
            character.transform.localScale = new Vector3(Mathf.Abs(character.transform.localScale.x), character.transform.localScale.y, character.transform.localScale.z);
        }
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null || character == null || rb == null) return;

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        if (character is RangerMonster)
        {
            if (distanceToPlayer <= character.MonsterData.attackRange)
            {
                stateManager.ChangeState(character.CreateAttackState());
                return;
            }
        }

        if (distanceToPlayer > character.MonsterData.detectRange)
        {
            stateManager.ChangeState(character.CreateIdleState());
            return;
        }

        UpdateMovement(distanceToPlayer);
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

    private void UpdateMovement(float distanceToPlayer)
    {
        Vector2 direction = (playerTransform.position - character.transform.position).normalized;
        float currentMoveSpeed = character.MoveSpeed;

        if (distanceToPlayer <= character.MonsterData.attackRange)
        {
            float clampedDistance = Mathf.Clamp01(distanceToPlayer / character.MonsterData.attackRange);
            currentMoveSpeed *= clampedDistance;
        }

        rb.velocity = direction * currentMoveSpeed;
    }
}
