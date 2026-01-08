using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image resultImage;
    [SerializeField] private Transform itemScrollContent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    [Header("Result Images")]
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite lossSprite;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }

    public void ShowResult(bool isVictory)
    {
        Debug.Log($"=== ShowResult 호출됨: {(isVictory ? "Victory" : "Loss")} ===");

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.PauseTimer();
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = isVictory ? "Victory" : "Loss";
        }

        if (resultImage != null)
        {
            resultImage.sprite = isVictory ? victorySprite : lossSprite;
            resultImage.enabled = true;
        }

        if (playTimeText != null && GameTimeManager.Instance != null)
        {
            string timeStr = GameTimeManager.Instance.GetFormattedTime();
            playTimeText.text = $"플레이 타임 {timeStr}";
        }

        if (totalGoldText != null && GameDataManager.Instance != null)
        {
            int gold = GameDataManager.Instance.GetTotalGold();
            totalGoldText.text = $"획득한 총골드 량 : {gold}G";
        }

        DisplayAcquiredItems();
    }

    private void DisplayAcquiredItems()
    {
        if (itemScrollContent == null || itemSlotPrefab == null || GameDataManager.Instance == null)
        {
            Debug.LogError("[ResultManager] 필수 참조가 null입니다!");
            return;
        }

        foreach (Transform child in itemScrollContent)
        {
            Destroy(child.gameObject);
        }

        List<AcquiredItemData> items = GameDataManager.Instance.GetAcquiredItems();
        Debug.Log($"[ResultManager] 획득 아이템 개수: {items.Count}");

        foreach (AcquiredItemData item in items)
        {
            if (item == null || item.itemIcon == null) continue;

            GameObject slotObj = Instantiate(itemSlotPrefab, itemScrollContent);
            ResultItemSlot slot = slotObj.GetComponent<ResultItemSlot>();

            if (slot != null)
            {
                slot.SetItem(item);
            }
        }
    }

    public void OnRetryClicked()
    {
        Debug.Log("[ResultManager] Retry 버튼 클릭됨");

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetAllData();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RetryGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnExitClicked()
    {
        Debug.Log("[ResultManager] Exit 버튼 클릭됨");

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetAllData();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoHome();
        }
        else
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
