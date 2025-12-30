using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BQueen : MeleeMonster
{
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float moveTime = 5f;

    [Header("일벌 소환 관련 필드")]
    [SerializeField] private BWorker bWorkerPrefab;
    [SerializeField] private int initialBWorkerCount;
    [SerializeField] private float spawnImpulseForce = 3f;
    [SerializeField] private float spawnDuration = 0.5f;
    [SerializeField] private float spawnRangeOffset = 0.5f;

    public bool isDamaged = false;

    private List<BWorker> bWorkers = new List<BWorker>();
    public float IdleTime => idleTime;
    public float MoveTime => moveTime;
    public float SpawnRangeOffset => spawnRangeOffset;

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

    public override void TakeDamage(int damage)
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

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector3 spawnPosition = transform.position + (Vector3)randomDirection * spawnRangeOffset;

        BWorker newBWorker = Instantiate(bWorkerPrefab, spawnPosition, Quaternion.identity);
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
            workerRb.AddForce(randomDirection * spawnImpulseForce, ForceMode2D.Impulse);
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
