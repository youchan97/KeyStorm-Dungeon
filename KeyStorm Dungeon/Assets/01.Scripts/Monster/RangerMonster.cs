using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMonster : Monster
{
    [SerializeField] private GameObject projectilePrefab;

    public override void Attack(Character character)
    {
        if (character == null) return;

        if (projectilePrefab != null)
        {
            
        }
        Debug.Log($"투사체 발사");
        //투사체 발사 로직
    }
}
