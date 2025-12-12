using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PassiveItemPickup : MonoBehaviour
{
    public ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        var stats = other.GetComponent<PlayerStats>();
        if (inv == null || stats == null) return;
        if (itemData == null) return;
        if (itemData.isActiveItem) return;

        inv.AddPassiveItem(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);
        ItemPopupUI.Instance.Show(itemData);
        Destroy(gameObject);
    }
}
