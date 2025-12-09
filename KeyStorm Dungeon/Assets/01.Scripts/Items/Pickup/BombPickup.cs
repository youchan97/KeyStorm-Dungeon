using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BombPickup : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!other.CompareTag("Player")) return;

        //var inv = other.GetComponent<PlayerInventory>();
        //if (inv == null) return;

        //inv.AddBomb(amount);
        //Destroy(gameObject);
    }
}
