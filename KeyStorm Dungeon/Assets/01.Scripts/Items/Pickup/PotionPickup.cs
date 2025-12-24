using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PotionPickup : MonoBehaviour
{
    public int healAmount;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();

        if (player == null) return;

        if (player.Hp == player.MaxHp) return;

        player.Heal(healAmount);
        Destroy(gameObject);
    }
}
