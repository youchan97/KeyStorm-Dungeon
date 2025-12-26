using TMPro;
using UnityEngine;

public class RangerMonster : Monster
{
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected Sprite bullet;


    private MonsterIdleState _idleState;
    private MonsterMoveState _moveState;
    private RangerMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new MonsterIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new MonsterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new RangerMonsterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MonsterDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Awake()
    {
        base.Awake();

        if (attackPoolManager == null)
        {
            attackPoolManager = FindObjectOfType<AttackPoolManager>();
            if (attackPoolManager == null)
            {
                Debug.LogError("RangerMonster: AttackPoolManager를 찾을 수 없음");
            }
        }
    }

    public override void Attack(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning($"{CharName}: Animation Event로 투사체 발사 시도했으나, CurrentAttackTarget이 null입니다. AttackState에서 타겟 설정을 확인하세요.", this);
            return;
        }

        if (attackPoolManager == null)
        {
            Debug.LogError("RangerMonster: AttackPoolManager가 할당되지 않아 투사체를 발사할 수 없음");
            return;
        }

        if (shootPoint == null)
        {
            shootPoint = this.transform;
        }

        AttackObj pooledAttackObj = attackPoolManager.GetAttack();
        if (pooledAttackObj == null)
        {
            Debug.LogError("오브젝트 풀에서 AttackObj를 가져오지 못함.");
            return;
        }

        Vector2 projectileDirection = (character.transform.position - shootPoint.position).normalized;

        float calculateProjectileLifeTime = 10f;

        pooledAttackObj.transform.position = shootPoint.position;
        pooledAttackObj.transform.rotation = Quaternion.identity;

        pooledAttackObj.InitData(bullet, Damage, projectileDirection, MonsterData.shotSpeed, calculateProjectileLifeTime, attackPoolManager, false, MonsterData.projectileColliderOffset, MonsterData.projectileColliderRadius);
    }
}
