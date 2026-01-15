using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BQueen : MeleeMonster
{
    [SerializeField] private float idleTime;
    [SerializeField] private float moveTime;

    [Header("일벌 소환 관련 필드")]
    [SerializeField] private BWorker bWorkerPrefab;
    [SerializeField] private int initialBWorkerCount;
    [SerializeField] private float spawnImpulseForce;
    [SerializeField] private float spawnDuration;
    [SerializeField] private float spawnRangeOffset;
    [SerializeField] private int maxSpawnAttempts;
    [SerializeField] private int maxSpawnQuantity;
    public bool isDamaged = false;

    private List<BWorker> bWorkers = new List<BWorker>();
    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float SpawnRangeOffset => spawnRangeOffset;
    public int MaxSpawnQuantity => maxSpawnQuantity;
    public List<BWorker> BWorkers => bWorkers;

    private BQueenIdleState _idleState;
    private BQueenMoveState _moveState;
    private BQueenAttackState _attackState;
    private BQueenDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new BQueenIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new BQueenMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new BQueenAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new BQueenDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Start()
    {
        base.Start();
        isDamaged = false;
        SpawnInitialBWorker();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        isDamaged = true;
    }

    public override void Die()
    {
        base.Die();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPlayer(collision);
    }

    private void SpawnInitialBWorker()
    {
        for (int i = 0; i < initialBWorkerCount; i++)
        {
            SpawnBWorker();
        }
    }

    public BWorker SpawnBWorker()
    {
        if (bWorkerPrefab == null) return null;

        Vector2 finalRandomDirection = Vector2.zero;
        Vector3 validSpawnPosition = transform.position;
        bool foundValidPosition = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            Vector3 spawnPosition = transform.position + (Vector3)randomDirection * spawnRangeOffset;

            if(IsSpawnPositionValid(spawnPosition, spawnCheckRadius, obstacleLayer))
            {
                finalRandomDirection = randomDirection;
                validSpawnPosition = spawnPosition;
                foundValidPosition = true;
                break;
            }
        }

        if (!foundValidPosition)
        {
            finalRandomDirection = Vector2.zero;
            validSpawnPosition = transform.position;
        }

        BWorker newBWorker = Instantiate(bWorkerPrefab, validSpawnPosition, Quaternion.identity);
        newBWorker.SetAssignedBQueen(this, spawnDuration);
        bWorkers.Add(newBWorker);

        if (MyRoom != null)
        {
            newBWorker.SetMyRoom(MyRoom);
            MyRoom.AddMonster(newBWorker);
        }

        Rigidbody2D workerRb = newBWorker.GetComponent<Rigidbody2D>();
        if (workerRb != null)
        {
            workerRb.AddForce(finalRandomDirection * spawnImpulseForce, ForceMode2D.Impulse);
        }

        return newBWorker;
    }

    public void RemoveWorker(BWorker worker)
    {
        if (bWorkers.Contains(worker))
        {
            bWorkers.Remove(worker);
        }
    }

    public void OnAttack()
    {
        SpawnBWorker();
    }
}
