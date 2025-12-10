using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public static HudUI Instance { get; private set; }

    [Header("텍스트 UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI bombText;

    [Header("액티브 아이템 UI")]
    public Image activeItemIcon;
    public TextMeshProUGUI activeItemNameText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 씬 전환해도 HUD 유지할 거면 켜두고, 아니면 주석처리해도 됩니다
        // DontDestroyOnLoad(gameObject);
    }
    // 이밑에 있는 코드들은 나중에 플레이어 상태,코인등 상태창이랑 연결하려고 쓰긴한건데 지워도 괜찮을것같습니다

    public void UpdateGold(int gold)
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    public void UpdateBomb(int bombCount)
    {
        if (bombText != null)
        {
            bombText.text = bombCount.ToString();
        }
    }

    public void SetActiveItem(ItemData data)
    {
        if (data == null)
        {
            if (activeItemNameText != null) activeItemNameText.text = "-";
            if (activeItemIcon != null) activeItemIcon.sprite = null;
            if (activeItemIcon != null) activeItemIcon.enabled = false;
            return;
        }

        if (activeItemNameText != null)
        {
            activeItemNameText.text = data.itemName;
        }

        // 아이템 아이콘을 나중에 ItemData에 추가하면 여기서 표시
        // public Sprite icon 을 ItemData에 추가한 뒤
        // if (activeItemIcon != null && data.icon != null)
        // {
        //     activeItemIcon.sprite = data.icon;
        //     activeItemIcon.enabled = true;
        // }
    }
}

