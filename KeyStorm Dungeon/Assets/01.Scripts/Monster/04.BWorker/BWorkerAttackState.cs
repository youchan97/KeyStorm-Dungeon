using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BWorkerAttackState : MonsterAttackState
{
    private BWorker bWorker;
    private Transform playerTransform;
    private float distanceToPlayer;

    private List<Node> currentPath;
    private int targetNodeIndex;
    private float nextPathUpdateTime;
    private float pathUpdateInterval = 0.1f;
    private float waypointReachThreshold = 0.1f;

    private Pathfinding roomPathfinding;

    public BWorkerAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.bWorker = character as BWorker;
    }

    public override void EnterState()
    {
        rb = character.MonsterRb;
        animator = character.Animator;
        playerTransform = character.PlayerTransform;

        animator.SetBool("IsAttack", true);

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
        if (character.PlayerGO == null)
        {
            character.ChangeStateToPlayerDied();
            return;
        }

        if (playerTransform == null) return;
        if (character == null || rb == null) return;

        distanceToPlayer = Vector2.Distance(character.transform.position, playerTransform.position);

        Move();

        character.FlipSprite(-rb.velocity.x);
    }

    public override void ExitState()
    {
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    public void Move()
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
            UpdateDirectionMovement(distanceToPlayer);
        }
    }

    public void InitializeRoomPathfindingForMonster()
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

    public void RequestNewPath()
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
    public void UpdateMovement()
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
    public void UpdateDirectionMovement(float distanceToPlayer)
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
