using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PotionPickup : MonoBehaviour
{
    public int healAmount = 2;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null || inv.stats == null) return;

        // 이미 풀피면 안 먹게 (원하면 삭제 가능)
        if (inv.stats.hp >= inv.stats.maxHp) return;

        inv.stats.Heal(healAmount);
        Destroy(gameObject);
    }
}
