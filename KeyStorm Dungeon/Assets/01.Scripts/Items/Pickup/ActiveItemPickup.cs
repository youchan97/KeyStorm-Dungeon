using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ActiveItemPickup : MonoBehaviour
{
    public ItemData itemData;

    private ItemPickupView view;
    private Collider2D col;

    private void Awake()
    {
        view = GetComponent<ItemPickupView>();
        col = GetComponent<Collider2D>();

        if (view != null)
            view.Apply(itemData);

        // 생성 직후 바로 다시 주워지는 문제 방지
        if (col != null)
            col.enabled = false;

        StartCoroutine(EnablePickupAfterDelay());
    }

    public void SetData(ItemData data)
    {
        itemData = data;
        if (view != null)
            view.Apply(data);
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        if (col != null)
            col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        if (itemData == null || !itemData.isActiveItem) return;

        inv.SetActiveItem(itemData);
        ItemPoolManager.Instance?.MarkAcquired(itemData);
        Destroy(gameObject);
    }
}
