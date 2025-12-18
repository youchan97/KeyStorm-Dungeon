using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class MonsterMoveState : CharacterMoveState<Monster>
{
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected List<Node> currentPath;
    protected int targetNodeIndex;
    protected float nextPathUpdateTime;
    protected float pathUpdateInterval = 0.1f;
    protected float waypointReachThreshold = 0.1f;

    public MonsterMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        animator.SetBool(MoveAnim, true);
        playerTransform = character.PlayerTransform;

        currentPath = null;
        targetNodeIndex = 0;
        RequestNewPath();
        nextPathUpdateTime = Time.time + pathUpdateInterval;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        if (playerTransform == null) return;

        if (character == null || rb == null) return;

        character.FlipSprite(rb.velocity.x);

        float distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        if (character is RangerMonster)
        {
            if (distanceToPlayer <= character.MonsterData.targetDistance)
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
            rb.velocity = Vector2.zero;
            //UpdateDirectionMovement(distanceToPlayer);
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

        currentPath = null;
        targetNodeIndex = 0;
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    protected void RequestNewPath()
    {
        if (PathfindingManager.Instance == null)
        {
            Debug.LogError("MonsterMoveState: 씬에 PathfindingManager이 없음");
            currentPath = null;
            return;
        }

        currentPath = PathfindingManager.Instance.RequestPath(character.transform.position, playerTransform.position, character.MonsterData.type);

        targetNodeIndex = 0;
    }
    
    protected void UpdateMovement()
    {
        if (currentPath == null || targetNodeIndex >= currentPath.Count)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector3 targetWorldPosition = PathfindingManager.Instance.Grid.GetWorldPointFromNode(currentPath[targetNodeIndex]);
        targetWorldPosition.z = character.transform.position.z;

        Vector2 direction = (targetWorldPosition - character.transform.position).normalized;

        float distanceToTargetNode = Vector2.Distance(character.transform.position, targetWorldPosition);

        if (distanceToTargetNode <= waypointReachThreshold)
        {
            targetNodeIndex++;
            if (targetNodeIndex >= currentPath.Count)
            {
                rb.velocity = Vector2.zero;
                currentPath = null;
                return;
            }
        }

        rb.velocity = direction * character.MoveSpeed;
    }

    protected void UpdateDirectionMovement(float distanceToPlayer)
    {
        Vector2 direction = (playerTransform.position - character.transform.position).normalized;
        float currentMoveSpeed = character.MoveSpeed;

        if (distanceToPlayer <= character.MonsterData.targetDistance)
        {
            float clampedDistance = Mathf.Clamp01(distanceToPlayer / character.MonsterData.targetDistance);
            currentMoveSpeed *= clampedDistance;
        }

        rb.velocity = direction * currentMoveSpeed;
    }
}
