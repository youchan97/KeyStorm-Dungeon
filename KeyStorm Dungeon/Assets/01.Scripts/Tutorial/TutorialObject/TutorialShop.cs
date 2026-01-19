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
            Debug.Log("[TutorialShop] 플레이어 진입");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
            Debug.Log("[TutorialShop] 플레이어 이탈");
        }
    }

    void Update()
    {
        if (currentPlayer == null) return;

        if (Input.GetKeyDown(KeyCode.E) && itemSlot != null && itemSlot.activeSelf)
        {
            BuyItem();
        }

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

            if (itemSlot != null)
            {
                itemSlot.SetActive(false);
            }

            TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnItemBought();
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

            currentPlayer.Inventory.AddBomb(1);

            if (bombSlot != null)
            {
                bombSlot.SetActive(false);
            }

            TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnBombBought();
            }
        }
        else
        {
            Debug.Log("[TutorialShop] 골드 부족!");
        }
    }
}
