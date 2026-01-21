using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BombPickup : MonoBehaviour
{
    public int amount = 1;

    [HideInInspector] public StoreSlot parentSlot;
    [HideInInspector] public int price;

    private bool isPickedUp = false;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.gravityScale = 0f;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }

        col.isTrigger = true;
        col.radius = 0.5f;
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

        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        /*if (price > 0 && !inv.TrySpendGold(price))
        {
            isPickedUp = false;
            return;
        }*/
        AudioManager.Instance.PlayEffect(GetBombSfx);
        inv.AddBomb(amount);

        if (parentSlot != null)
        {
            parentSlot.Clear();
        }
        else
        {
            ItemDropManager.Instance?.ReturnBombPickup(this);
        }
    }
}
