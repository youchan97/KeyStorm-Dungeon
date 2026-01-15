using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialShop : MonoBehaviour
{
    [Header("Shop Items")]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject bombSlot;
    [SerializeField] private int itemPrice = 50;
    [SerializeField] private int bombPrice = 30;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI bombPriceText;

    private Player currentPlayer;

    void Start()
    {
        if (itemPriceText != null)
        {
            itemPriceText.text = $"{itemPrice}G";
        }

        if (bombPriceText != null)
        {
            bombPriceText.text = $"{bombPrice}G";
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<Player>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
        }
    }

    void Update()
    {
        if (currentPlayer == null) return;

        // E 키로 아이템 구매
        if (Input.GetKeyDown(KeyCode.E) && itemSlot != null && itemSlot.activeSelf)
        {
            BuyItem();
        }

        // Q 키로 폭탄 구매
        if (Input.GetKeyDown(KeyCode.Q) && bombSlot != null && bombSlot.activeSelf)
        {
            BuyBomb();
        }
    }

    void BuyItem()
    {
        if (currentPlayer.Inventory.TrySpendGold(itemPrice))
        {
            Debug.Log("[TutorialShop] 아이템 구매 성공!");

            // 아이템 지급 (실제 게임에서는 아이템 데이터 전달)
            // currentPlayer.Inventory.AddPassiveItem(itemData);

            // 슬롯 비활성화
            if (itemSlot != null)
            {
                itemSlot.SetActive(false);
            }

            // TutorialManager에 알림
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnItemBought();
            }
        }
        else
        {
            Debug.Log("[TutorialShop] 골드 부족!");
        }
    }

    void BuyBomb()
    {
        if (currentPlayer.Inventory.TrySpendGold(bombPrice))
        {
            Debug.Log("[TutorialShop] 폭탄 구매 성공!");

            // 폭탄 지급
            currentPlayer.Inventory.AddBomb(1);

            // 슬롯 비활성화
            if (bombSlot != null)
            {
                bombSlot.SetActive(false);
            }

            // TutorialManager에 알림
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnBombBought();
            }
        }
        else
        {
            Debug.Log("[TutorialShop] 골드 부족!");
        }
    }
}
