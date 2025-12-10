using UnityEngine;

public class RangerMonster : Monster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private AttackPoolManager attackPoolManager;

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
        if (character == null) return;
        Transform targetPosition = character.transform;

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

        MonsterProjectile monsterProjectile = pooledAttackObj.GetComponent<MonsterProjectile>();

        if ( monsterProjectile != null)
        {
            pooledAttackObj.transform.position = shootPoint.position;
            pooledAttackObj.transform.rotation = Quaternion.identity;

            monsterProjectile.Initialize(targetPosition, MonsterData.shotSpeed, MonsterData.characterData.damage, attackPoolManager);
            Debug.Log($"투사체 발사");
        }
        else
        {
            attackPoolManager.ReturnPool(pooledAttackObj);
        }
    }
}
