using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PotionPickup : MonoBehaviour
{
    [Header("회복량")]
    public int healAmount = 2;

    [HideInInspector] public StoreSlot parentSlot;
    [HideInInspector] public int price;

    private CircleCollider2D col;
    private Rigidbody2D rb;
    private bool isPickedUp = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.gravityScale = 0f;

        col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }

        col.isTrigger = true;
        //col.radius = 0.5f;
    }

    private void Reset()
    {
        CircleCollider2D c = GetComponent<CircleCollider2D>();
        if (c != null)
        {
            c.isTrigger = true;
            c.radius = 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;
        TryPickup(player);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isPickedUp) return;
        Player player = other.GetComponent<Player>();
        if (player == null) return;
        TryPickup(player);
    }

    public void TryPickup(Player player)
    {    
        if (isPickedUp) return;
        isPickedUp = true;

        if (player.Hp >= player.MaxHp) return;

        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        /*if (price > 0 && !inv.TrySpendGold(price))
        {
            isPickedUp = false;
            return;
        }*/

        player.Heal(healAmount);

        if (inv != null && price > 0)
        {
            inv.AddPotion(1);
        }

        if (parentSlot != null)
        {
            parentSlot.Clear();
        }

        Destroy(gameObject);
    }
}
