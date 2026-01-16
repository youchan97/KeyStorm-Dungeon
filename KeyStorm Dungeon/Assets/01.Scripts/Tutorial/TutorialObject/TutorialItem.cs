using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string itemName = "테스트 아이템";

    [Header("UI")]
    [SerializeField] private GameObject infoPanel;

    void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // 아이템 획득 처리
                if (itemData != null)
                {
                    player.Inventory.AddPassiveItem(itemData);
                }

                Debug.Log($"[TutorialItem] 아이템 획득: {itemName}");

                // TutorialManager에 알림
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnItemPickedUp();
                }

                Destroy(gameObject);
            }
        }
    }

    // 가까이 갔을 때 정보 표시
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
