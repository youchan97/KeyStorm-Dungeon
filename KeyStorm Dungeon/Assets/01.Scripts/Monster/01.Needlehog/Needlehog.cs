using System;
using UnityEngine;

public class Needlehog : RangerMonster
{
    [SerializeField] private float idleTime = 1.0f;
    [SerializeField] private float moveTime = 3.0f;
    [SerializeField] private float projectileLifeTime = 5f;

    public float IdleTime => idleTime;
    public float MoveTime => moveTime;

    private NeedlehogIdleState _idleState;
    private NeedlehogMoveState _moveState;
    private NeedlehogAttackState _attackState;
    private NeedlehogDieState _dieState;

    public event Action<Collision2D> OnWallOrCollisionHit;
    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new NeedlehogIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new NeedlehogMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new NeedlehogAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new NeedlehogDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        if(MonsterStateManager.CurState != _attackState && MonsterStateManager.CurState != _dieState)
        {
            MonsterStateManager.ChangeState(CreateAttackState());
        }
    }

    public void OnAttack()
    {
        if (attackPoolManager == null) return;

        if (shootPoint == null)
        {
            shootPoint = this.transform;
        }

        Vector2[] directions = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

        float[] rotations = { 0f, 180f, 90f, -90f };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 dir = directions[i];
            Quaternion rotation = Quaternion.Euler(0, 0, rotations[i]);

            AttackObj pooledAttackObj = attackPoolManager.GetAttack();
            if (pooledAttackObj == null)
            {
                Debug.LogError("오브젝트 풀에서 AttackObj를 가져오지 못함.");
                continue;
            }

            pooledAttackObj.transform.position = shootPoint.position;
            pooledAttackObj.transform.rotation = rotation;

            pooledAttackObj.InitData(bullet, Damage, dir, MonsterData.shotSpeed, projectileLifeTime, AttackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Collision"))
        {
            OnWallOrCollisionHit?.Invoke(collision);
        }
    }
}
