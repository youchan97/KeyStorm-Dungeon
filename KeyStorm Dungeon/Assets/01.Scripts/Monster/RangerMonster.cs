using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMonster : Monster
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    public override void Attack(Character character)
    {
        if (character == null) return;
        Transform characterPosition = character.transform;

        GameObject projectileObj = projectilePrefab;
        projectileObj.transform.position = shootPoint.position;
        projectileObj.transform.rotation = Quaternion.identity;
        
        if (projectilePrefab != null)
        {
            MonsterProjectile monsterProjectile = projectilePrefab.GetComponent<MonsterProjectile>();
            if (monsterProjectile != null)
            {
                monsterProjectile.Initialize(characterPosition, MonsterData.shotSpeed, MonsterData.characterData.damage, projectilePrefab);
                Debug.Log($"투사체 발사");
            }
        }
    }
}
