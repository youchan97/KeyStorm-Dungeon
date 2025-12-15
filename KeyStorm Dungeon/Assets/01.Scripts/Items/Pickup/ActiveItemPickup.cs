using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ActiveItemPickup : MonoBehaviour
{
    public ItemData itemData;  // isActiveItem == true

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null || itemData == null) return;
        if (!itemData.isActiveItem) return;

        if (inv.activeItem != null)
        {
            DropOldActive(inv.activeItem, transform.position);
        }

        inv.SetActiveItem(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);
        ItemPopupUI.Instance.Show(itemData);
        Destroy(gameObject);
    }

    void DropOldActive(ItemData oldItem, Vector3 position)
    {
        GameObject prefab = ItemDatabase.Instance.GetActivePickupPrefab(oldItem.itemId);
        if (prefab == null) return;

        GameObject drop = Instantiate(prefab, position, Quaternion.identity);
        var pickup = drop.GetComponent<ActiveItemPickup>();
        pickup.itemData = oldItem;
    }
}
