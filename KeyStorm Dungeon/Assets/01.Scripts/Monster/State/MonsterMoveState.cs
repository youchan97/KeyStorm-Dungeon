using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    private Pathfinding roomPathfinding;

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

        InitializeRoomPathfindingForMonster();

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

        // 임시로 플레이어의 사망을 체크
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
        }

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

        Move();
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
        roomPathfinding = null;
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    protected void Move()
    {
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

    protected void InitializeRoomPathfindingForMonster()
    {
        if (PathfindingManager.Instance == null)
        {
            return;
        }

        Room myRoom = character.MyRoom;

        if (myRoom == null) return;

        Tilemap roomGround = myRoom.GetRoomGroundTilemap();
        Tilemap roomObstacleGround = null;
        Tilemap roomObstacleAir = null;
        Tilemap roomObstacleUniversal = myRoom.GetRoomWallTilemap();

        roomPathfinding = PathfindingManager.Instance.CreateRoomPathfinding(roomGround, roomObstacleGround, roomObstacleAir, roomObstacleUniversal);

        if (roomPathfinding == null)
        {
            Debug.LogError($"MonsterMoveState: Pathfinding 객체 생성 실패");
        }
    }

    protected void RequestNewPath()
    {
        if (PathfindingManager.Instance == null)
        {
            Debug.LogError("MonsterMoveState: 씬에 PathfindingManager이 없음");
            currentPath = null;
            return;
        }

        currentPath = roomPathfinding.FindPath(character.transform.position, playerTransform.position, character.MonsterData.type);
        targetNodeIndex = 0;
    }
    
    // 플레이어에게 접근하는 움직임 (a* 적용)
    protected void UpdateMovement()
    {
        if (currentPath == null || targetNodeIndex >= currentPath.Count)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector3 targetWorldPosition = roomPathfinding.Grid.GetWorldPointFromNode(currentPath[targetNodeIndex]);
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

    // 플레이어에게 접근하는 움직임 (a* 미적용)
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
