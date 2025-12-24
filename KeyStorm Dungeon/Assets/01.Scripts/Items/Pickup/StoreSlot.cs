using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class StoreSlot : MonoBehaviour
{
    public enum ConsumableType 
    { 
        None, 
        Bomb, 
        Potion 
    }


    [Header("진열 위치(없으면 자기 위치)")]
    public Transform spawnPoint;

    [Header("가격 텍스트(TMP) - 없으면 자식에서 자동 탐색")]
    public TMP_Text priceText;

    [Header("가격")]
    public int price;

    // ===== 내부 상태 =====
    private GameObject spawnedObj;

    // 아이템용
    private bool isItemProduct;
    private ItemData itemData;

    // 소모품용
    private ConsumableType type;
    private int amount;
    private int healAmount;

    private void Awake()
    {
        // 슬롯 콜라이더는 트리거
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // PriceText 자동 찾기(비활성 포함)
        if (priceText == null)
            priceText = GetComponentInChildren<TMP_Text>(true);
    }

    // ================================
    // 가격 UI
    // ================================
    private void ShowPrice(int playerGold = -1)
    {
        if (priceText == null) return;

        priceText.gameObject.SetActive(true);
        priceText.text = price.ToString();

        if (playerGold >= 0 && playerGold < price)
            priceText.color = Color.red;
        else
            priceText.color = Color.yellow;
    }

    private void HidePrice()
    {
        if (priceText == null) return;
        priceText.gameObject.SetActive(false);
    }

    // ================================
    // 진열 오브젝트를 "절대 먹히지 않게" 만들기
    // (상점은 StoreSlot만 구매 처리)
    // ================================
    private void MakeDisplayNonPickup(GameObject obj)
    {
        if (obj == null) return;

        //모든 Collider2D 비활성화(자식 포함) -> 공짜 픽업 원천 차단
        foreach (var c in obj.GetComponentsInChildren<Collider2D>(true))
            c.enabled = false;

        //Rigidbody2D도 있으면 물리 반응 차단
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody2D>(true))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 혹시 Pickup 스크립트가 붙어 있어도 작동 못하게 끄기
        // (아이템 프리팹을 그대로 진열해도 안전)
        foreach (var mb in obj.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is PassiveItemPickup || mb is ActiveItemPickup)
                mb.enabled = false;
        }
    }

    // ================================
    // 아이템 진열
    // ================================
    public void SetItemProduct(GameObject pickupPrefab, ItemData data, int _price)
    {
        Clear();

        isItemProduct = true;
        itemData = data;
        price = _price;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        // 상점 진열은 부모로 붙일 필요 없음(부모 이슈 방지)
        spawnedObj = Instantiate(pickupPrefab, point.position, Quaternion.identity);

        // 공짜 픽업 방지
        MakeDisplayNonPickup(spawnedObj);

        // 스프라이트 적용(보이기용)
        spawnedObj.GetComponent<ItemPickupView>()?.Apply(data);

        ShowPrice();
    }

    // ================================
    // 소모품 진열 (폭탄/포션)
    // ================================
    public void SetConsumableProduct(GameObject displayPrefab, int _price, ConsumableType _type, int _amount, int _healAmount = 0)
    {
        Clear();

        isItemProduct = false;
        itemData = null;

        price = _price;
        type = _type;
        amount = _amount;
        healAmount = _healAmount;

        Transform point = spawnPoint != null ? spawnPoint : transform;
        spawnedObj = Instantiate(displayPrefab, point.position, Quaternion.identity);

        // 폭탄/포션 공짜 픽업 방지 (가장 중요)
        MakeDisplayNonPickup(spawnedObj);

        ShowPrice();
    }

    // ================================
    // 구매 처리
    // ================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        if (inv == null) return;

        // 진열된 물건 없으면 무시
        if (spawnedObj == null) return;

        if (price <= 0)
        {
            Debug.LogWarning("[StoreSlot] price가 0 이하라 구매를 막음", this);
            return;
        }

        if (!inv.TrySpendGold(price))
        {
            Debug.Log($"돈부족 need={price}, have={inv.gold}", this);
            ShowPrice(inv.gold);
            return;
        }

        if (isItemProduct)
        {
            if (itemData == null) { Clear(); return; }

            if (itemData.isActiveItem) inv.SetActiveItem(itemData);
            else inv.AddPassiveItem(itemData);

            ItemPoolManager.Instance?.MarkAcquired(itemData);
            Clear();
        }
        else
        {
            switch (type)
            {
                case ConsumableType.Bomb:
                    inv.AddBomb(amount);
                    break;

                case ConsumableType.Potion:
                    inv.AddPotion(amount); 
                    break;
            }
            Clear();
        }
    }

    // ================================
    // 정리
    // ================================
    public void Clear()
    {
        if (spawnedObj != null) Destroy(spawnedObj);
        spawnedObj = null;

        isItemProduct = false;
        itemData = null;

        price = 0;
        type = ConsumableType.None;
        amount = 0;
        healAmount = 0;

        HidePrice();
    }
}
