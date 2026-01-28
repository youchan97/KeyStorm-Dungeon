using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ConstValue;

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

    private GameObject spawnedObj;

    private bool isItemProduct;
    private ItemData itemData;

    private ConsumableType type;
    private int amount;
    private int healAmount;

    AudioManager audioManager;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (priceText == null)
            priceText = GetComponentInChildren<TMP_Text>(true);

        audioManager = AudioManager.Instance;
    }

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

    private void MakeDisplayNonPickup(GameObject obj)
    {
        if (obj == null) return;

        var passivePickup = obj.GetComponent<PassiveItemPickup>();
        var activePickup = obj.GetComponent<ActiveItemPickup>();
        var potionPickup = obj.GetComponent<PotionPickup>();
        var bombPickup = obj.GetComponent<BombPickup>();

        bool isItemPickup = (passivePickup != null || activePickup != null);
        bool isConsumable = (potionPickup != null || bombPickup != null);

        if (isItemPickup)
        {
            CircleCollider2D circleCol = obj.GetComponent<CircleCollider2D>();
            if (circleCol != null)
            {
                circleCol.enabled = true;
                circleCol.isTrigger = true;
                //circleCol.radius = 1.5f;
            }

            foreach (var rb in obj.GetComponentsInChildren<Rigidbody2D>(true))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (passivePickup != null)
            {
                passivePickup.enabled = true;
                passivePickup.isShopDisplay = true;
            }

            if (activePickup != null)
            {
                activePickup.enabled = true;
                activePickup.isShopDisplay = true;
            }
        }
        else if (isConsumable)
        {
            foreach (var c in obj.GetComponentsInChildren<Collider2D>(true))
            {
                c.enabled = true;
                c.isTrigger = true;
            }

            foreach (var rb in obj.GetComponentsInChildren<Rigidbody2D>(true))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (potionPickup != null)
            {
                potionPickup.enabled = true;
            }

            if (bombPickup != null)
            {
                bombPickup.enabled = true;
            }
        }
        else
        {
            foreach (var c in obj.GetComponentsInChildren<Collider2D>(true))
            {
                c.enabled = false;
            }

            foreach (var rb in obj.GetComponentsInChildren<Rigidbody2D>(true))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            foreach (var mb in obj.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (mb is ItemPickupView) continue;

                var mbType = mb.GetType();
                if (mbType.Name.Contains("Pickup") || mbType.Name.Contains("Drop"))
                {
                    mb.enabled = false;
                }
            }
        }
    }

    public void SetItemProduct(GameObject pickupPrefab, ItemData data, int _price)
    {
        Clear();

        isItemProduct = true;
        itemData = data;
        price = _price;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        spawnedObj = Instantiate(pickupPrefab, point.position, Quaternion.identity);

        var passivePickup = spawnedObj.GetComponent<PassiveItemPickup>();
        if (passivePickup != null)
        {
            passivePickup.itemData = data;
        }

        var activePickup = spawnedObj.GetComponent<ActiveItemPickup>();
        if (activePickup != null)
        {
            activePickup.itemData = data;
            activePickup.SetData(data);
        }

        MakeDisplayNonPickup(spawnedObj);

        spawnedObj.GetComponent<ItemPickupView>()?.Apply(data);

        ShowPrice();
    }

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

        MakeDisplayNonPickup(spawnedObj);

        CircleCollider2D col = spawnedObj.GetComponent<CircleCollider2D>();
        if (col != null)
            col.enabled = false;

        var potionPickup = spawnedObj.GetComponent<PotionPickup>();
        if (potionPickup != null)
        {
            potionPickup.parentSlot = this;
            potionPickup.price = _price;
        }

        var bombPickup = spawnedObj.GetComponent<BombPickup>();
        if (bombPickup != null)
        {
            bombPickup.parentSlot = this;
            bombPickup.price = _price;
        }

        ShowPrice();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>();
        Player player = other.GetComponent<Player>();
        if (inv == null || player == null) return;

        if (spawnedObj == null) return;

        if (price <= 0)
        {
            Debug.LogWarning("[StoreSlot] price가 0 이하라 구매를 막음", this);
            return;
        }

        //if (!isItemProduct) return;
        if (price <= 0 && inv.gold < price)
        {
            Debug.Log($"돈부족 need={price}, have={inv.gold}", this);
            ShowPrice(inv.gold);
        }else{
            switch (type)
            {
                case ConsumableType.Potion:
                    if (player.Hp == player.MaxHp) return;
                    inv.TrySpendGold(price);
                    PotionPickup potion = spawnedObj.GetComponent<PotionPickup>();
                    if (player.Hp >= player.MaxHp || potion == null)
                        return;
                    potion.TryPickup(player);
                    break;
                case ConsumableType.Bomb:
                    inv.TrySpendGold(price);
                    BombPickup bomb = spawnedObj.GetComponent<BombPickup>();
                    if (bomb != null)
                        bomb.TryPickup(player);
                    break;
                default:
                    break;
            }
        }



        if (itemData == null)
            return;

        if (itemData.isActiveItem)
        {
            inv.SetActiveItem(itemData);
        }
        else
        {
            inv.AddPassiveItem(itemData);
            player.UpdatePlayerData(itemData);
        }
        audioManager.PlayEffect(GetItemSfx);
        ItemPoolManager.Instance?.MarkAcquired(itemData);

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        hook?.ReportItemPurchase();

        Clear();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player == null)
            return;


        if(priceText.color == Color.red)
            priceText.color = Color.yellow;
    }

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
