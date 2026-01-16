using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldItemUIWidget : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text tierText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image background;

    [Header("티어별 배경")]
    [SerializeField] private Sprite tier0Background;
    [SerializeField] private Sprite tier1Background;
    [SerializeField] private Sprite tier2Background;
    [SerializeField] private Sprite tier3Background;
    [SerializeField] private Sprite tier4Background;

    [Header("티어별 색상")]
    [SerializeField] private Color tier0Color = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color tier1Color = Color.white;
    [SerializeField] private Color tier2Color = new Color(0.3f, 0.8f, 1f);
    [SerializeField] private Color tier3Color = new Color(0.8f, 0.3f, 1f);
    [SerializeField] private Color tier4Color = new Color(1f, 0.8f, 0.2f);

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Camera uiCamera;

    private Transform trackingTarget;
    private Vector3 trackingOffset;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            uiCamera = (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? (canvas.worldCamera ?? Camera.main)
                : null;
        }

        if (background != null)
        {
            Color bgColor = background.color;
            //bgColor.a = 0.85f;
            background.color = bgColor;
        }
    }

    public void Bind(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[WorldItemUIWidget] 바인딩할 데이터가 없습니다.");
            return;
        }

        Debug.Log($"[Widget] 바인딩 시작: {data.itemName}");

        // 1. 아이템 이름
        if (itemNameText != null)
        {
            itemNameText.text = string.Format("{0}\n{1}", data.itemName, data.itemEnglishName);
            itemNameText.color = GetTierColor(data.tier);
            Debug.Log($"[Widget] 이름 설정: {data.itemName}");
        }
        else
        {
            Debug.LogError("[Widget] itemNameText가 null입니다!");
        }

        // 2. 아이콘
        if (itemIconImage != null)
        {
            if (data.iconSprite != null)
            {
                itemIconImage.sprite = data.iconSprite;
                itemIconImage.gameObject.SetActive(true);
            }
            else
            {
                itemIconImage.gameObject.SetActive(false);
            }
        }


        if(tierText != null)
        {
            tierText.text = string.Format("Tier : {0}", GetTierText(data.tier));
            tierText.color = GetTierColor(data.tier);
        }

        // 3. 스탯
        if (statsText != null)
        {
            statsText.text = BuildStatsText(data);
            Debug.Log($"[Widget] 스탯 설정 완료");
        }
        else
        {
            Debug.LogError("[Widget] statsText가 null입니다!");
        }

        // 4. 설명
        if (descriptionText != null)
        {
            descriptionText.text = BuildDescriptionText(data);
        }

        // 5. 배경
        if (background != null)
        {
            Sprite tierBg = GetTierBackground(data.tier);
            if (tierBg != null)
            {
                background.sprite = tierBg;
            }
        }
    }

    string GetTierText(ItemTier tier)
    {
        return tier switch
        {
            ItemTier.Tier0 => "Common",
            ItemTier.Tier1 => "Uncommon",
            ItemTier.Tier2 => "Rare",
            ItemTier.Tier3 => "Epic",
            ItemTier.Tier4 => "Legendary",
            _ => "null"
        };
    }

    private string BuildStatsText(ItemData data)
    {
        StringBuilder sb = new StringBuilder();

        if (data.maxHp != 0)
            sb.AppendLine(FormatStat("체력", data.maxHp));

        if (!Mathf.Approximately(data.moveSpeed, 0f))
            sb.AppendLine(FormatStat("이동속도", data.moveSpeed));

        if (!Mathf.Approximately(data.damage, 0f))
            sb.AppendLine(FormatStat("공격력", data.damage));

        if (!Mathf.Approximately(data.damageMultiple, 0f))
            sb.AppendLine(FormatStatPercent("공격력 배율", data.damageMultiple));

        if (!Mathf.Approximately(data.specialDamageMultiple, 0f))
            sb.AppendLine(FormatStatPercent("특수 공격력", data.specialDamageMultiple));

        if (!Mathf.Approximately(data.attackSpeed, 0f))
            sb.AppendLine(FormatStat("공격속도", data.attackSpeed));

        if (!Mathf.Approximately(data.attackSpeedMultiple, 0f))
            sb.AppendLine(FormatStatPercent("공격속도 배율", data.attackSpeedMultiple));

        if (!Mathf.Approximately(data.range, 0f))
            sb.AppendLine(FormatStat("사거리", data.range));

        if (!Mathf.Approximately(data.shotSpeed, 0f))
            sb.AppendLine(FormatStat("탄속", data.shotSpeed));

        if (data.maxAmmo != 0)
            sb.AppendLine(FormatStat("최대 탄창", data.maxAmmo));

        if (data.useAmmo != 0)
            sb.AppendLine(FormatStat("탄약 소모", data.useAmmo));

        if (!Mathf.Approximately(data.scale, 0f))
            sb.AppendLine(FormatStatPercent("크기", data.scale));

        return sb.ToString().TrimEnd();
    }

    private string FormatStat(string statName, float value)
    {
        string colorTag = value > 0 ? "<color=#00FF00>" : "<color=#FF0000>";
        string sign = value > 0 ? "+" : "";

        if (Mathf.Approximately(value, Mathf.Round(value)))
        {
            return $"• {colorTag}{statName} {sign}{(int)value}</color>";
        }
        else
        {
            return $"• {colorTag}{statName} {sign}{value:F1}</color>";
        }
    }

    private string FormatStat(string statName, int value)
    {
        string colorTag = value > 0 ? "<color=#00FF00>" : "<color=#FF0000>";
        string sign = value > 0 ? "+" : "";

        return $"• {colorTag}{statName} {sign}{value}</color>";
    }

    private string FormatStatPercent(string statName, float value)
    {
        string colorTag = value > 0 ? "<color=#00FF00>" : "<color=#FF0000>";
        string sign = value > 0 ? "+" : "";
        float percentage = value * 100f;

        return $"• {colorTag}{statName} {sign}{percentage:F0}%</color>";
    }

    private string BuildDescriptionText(ItemData data)
    {
        StringBuilder sb = new StringBuilder();

        if (data.attackChange)
        {
            sb.AppendLine("<color=#FFD700> 공격 방식 변화</color>");
        }

        if (data.isActiveItem)
        {
            sb.AppendLine("<color=#00BFFF> 액티브 스킬</color>");

            switch (data.cooldownType)
            {
                case ActiveCooldownType.PerRoom:
                    sb.AppendLine($"  쿨다운: 방 클리어 {data.cooldownMax}회");
                    break;
                case ActiveCooldownType.PerTime:
                    sb.AppendLine($"  쿨다운: {data.cooldownMax}초");
                    break;
            }
        }

        if (!string.IsNullOrEmpty(data.description))
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append(data.description);
        }

        return sb.ToString();
    }

    private Color GetTierColor(ItemTier tier)
    {
        switch (tier)
        {
            case ItemTier.Tier0: return tier0Color;
            case ItemTier.Tier1: return tier1Color;
            case ItemTier.Tier2: return tier2Color;
            case ItemTier.Tier3: return tier3Color;
            case ItemTier.Tier4: return tier4Color;
            default: return Color.white;
        }
    }

    private Sprite GetTierBackground(ItemTier tier)
    {
        switch (tier)
        {
            case ItemTier.Tier0: return tier0Background;
            case ItemTier.Tier1: return tier1Background;
            case ItemTier.Tier2: return tier2Background;
            case ItemTier.Tier3: return tier3Background;
            case ItemTier.Tier4: return tier4Background;
            default: return null;
        }
    }

    public void SetTrackingTarget(Transform target, Vector3 offset)
    {
        trackingTarget = target;
        trackingOffset = offset;
    }

    public void ClearTrackingTarget()
    {
        trackingTarget = null;
    }

    private void LateUpdate()
    {
        if (trackingTarget == null) return;
        if (canvas == null || canvasRect == null || rectTransform == null) return;

        Vector3 worldPos = trackingTarget.position + trackingOffset;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, uiCamera, out localPos);

        rectTransform.anchoredPosition = localPos;
    }
}
