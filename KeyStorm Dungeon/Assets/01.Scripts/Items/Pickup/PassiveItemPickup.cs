using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PassiveItemPickup : MonoBehaviour
{
    public ItemData itemData;
    private ItemPickupView view;

    private void Awake()
    {
        view = GetComponent<ItemPickupView>();
        if (view != null) view.Apply(itemData);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;
        if (itemData == null) return;
        if (itemData.isActiveItem) return;

        FindObjectOfType<InventoryModel>()?.AddItem(itemData);
        FindObjectOfType<InventoryUIController>()?.Refresh(); // 인벤 열려있을 때 즉시 반영하고 싶으면

        inv.AddPassiveItem(itemData);
        player.PlayerStatUpdate(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);
        Destroy(gameObject);
    }

    IEnumerator DisableTriggerTemporarily(Collider2D col)
    {
        col.enabled = false;
        yield return new WaitForSeconds(0.3f);
        col.enabled = true;
    }
}
