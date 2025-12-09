using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonster : Monster
{

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if (CurrentAttackCooldown <= 0f)
        {
            if (currentGameObject.CompareTag("Player"))
            {
                Player player = currentGameObject.GetComponent<Player>();

                if (player != null)
                {
                    Attack(player);
                    Debug.Log("공격!");
                    ResetAttackCooldown();
                }
            }

        }
    }
}
