using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDescriptionView : MonoBehaviour
{
    [Header("Theme")]
    [SerializeField] private ItemUIThemeSO themeSO;

    [Header("UI Refs")]
    [SerializeField] private Image frameImage; // 티어 프레임 Image
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Stat Lines")]
    [SerializeField] private Transform statLinesRoot; // VerticalLayoutGroup 권장
    [SerializeField] private TMP_Text statLinePrefab; // 한 줄 TMP 프리팹

    [Header("Extra Desc")]
    [SerializeField] private TMP_Text extraDescText;

    [Header("Tier Effect Root")]
    [SerializeField] private Transform tierEffectRoot;

    private readonly List<TMP_Text> _spawnedLines = new();
    private GameObject _spawnedEffect;

    public void SetItem(ItemData data)
    {
        if (data == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        // Theme
        ApplyTheme(data.tier);

        // Basic UI
        if (nameText) nameText.text = data.itemName;

        if (iconImage)
        {
            iconImage.sprite = data.iconSprite;
            iconImage.enabled = data.iconSprite != null;
        }

        //Stat lines
        ClearLines();
        BuildStatLinesFromItemData(data);

        // Extra desc (설명)
        if (extraDescText)
            extraDescText.text = data.description ?? "";
    }

    private void Hide()
    {
        ClearLines();
        if (_spawnedEffect) Destroy(_spawnedEffect);
        _spawnedEffect = null;
        gameObject.SetActive(false);
    }

    private void ApplyTheme(ItemTier tier)
    {
        if (themeSO == null) return;
        var theme = themeSO.Get(tier);
        if (theme == null) return;

        if (frameImage != null && theme.frameSprite != null)
            frameImage.sprite = theme.frameSprite;

        if (_spawnedEffect) Destroy(_spawnedEffect);
        _spawnedEffect = null;

        if (theme.effectPrefab != null && tierEffectRoot != null)
            _spawnedEffect = Instantiate(theme.effectPrefab, tierEffectRoot);
    }

    private void ClearLines()
    {
        for (int i = 0; i < _spawnedLines.Count; i++)
        {
            if (_spawnedLines[i] != null)
                Destroy(_spawnedLines[i].gameObject);
        }
        _spawnedLines.Clear();
    }

    private void AddLine(string text, bool positive)
    {
        if (statLinePrefab == null || statLinesRoot == null) return;

        var line = Instantiate(statLinePrefab, statLinesRoot);
        line.text = text;
        line.color = positive ? Color.green : Color.red;
        _spawnedLines.Add(line);
    }

    private void BuildStatLinesFromItemData(ItemData d)
    {
        // 규칙:
        // - 0이면 표시 안 함
        // - +면 초록 / -면 빨강
        // - multiple(배수)는 x1.10 형태로 표기 (1.0이 기본이라고 가정)
        // -multiple 값이 "증가량"인지 "최종 배수"인지 팀 규칙에 따라 다를 수 있음 (여기선 값 그대로 x표기)
        // - 만약 너희가 0.2를 "20% 증가"로 쓰면 아래만 살짝 바꿔주면 됨

        if (d.moveSpeed != 0f) AddLine($"MoveSpeed {(d.moveSpeed > 0 ? "+" : "")}{d.moveSpeed:0.##}", d.moveSpeed > 0);

        if (d.maxHp != 0) AddLine($"MaxHP {(d.maxHp > 0 ? "+" : "")}{d.maxHp}", d.maxHp > 0);

        if (d.damage != 0f) AddLine($"Damage {(d.damage > 0 ? "+" : "")}{d.damage:0.##}", d.damage > 0);

        if (d.attackSpeed != 0f) AddLine($"AttackSpeed {(d.attackSpeed > 0 ? "+" : "")}{d.attackSpeed:0.##}", d.attackSpeed > 0);

        if (d.range != 0f) AddLine($"Range {(d.range > 0 ? "+" : "")}{d.range:0.##}", d.range > 0);

        if (d.shotSpeed != 0f) AddLine($"ShotSpeed {(d.shotSpeed > 0 ? "+" : "")}{d.shotSpeed:0.##}", d.shotSpeed > 0);

        if (d.maxAmmo != 0) AddLine($"MaxAmmo {(d.maxAmmo > 0 ? "+" : "")}{d.maxAmmo}", d.maxAmmo > 0);

        if (d.useAmmo != 0) AddLine($"UseAmmo {(d.useAmmo > 0 ? "+" : "")}{d.useAmmo}", d.useAmmo < 0);
        // useAmmo는 소모가 줄면 좋은 것일 수 있어서(예: -1이 버프)
        // 팀 규칙에 맞게 positive 판정 바꾸면 됨
        // 지금은 값이 작아질수록 좋다고 가정 - (d.useAmmo < 0)이면 초록

        if (d.scale != 0f) AddLine($"Scale {(d.scale > 0 ? "+" : "")}{d.scale:0.##}", d.scale > 0);

        // 배수 계열(값이 1.0이 기준인지 0.0이 기준인지 불확실함)
        if (d.damageMultiple != 0f)
        {
            bool pos = d.damageMultiple >= 1f;
            AddLine($"Damage x{d.damageMultiple:0.##}", pos);
        }

        if (d.attackSpeedMultiple != 0f)
        {
            bool pos = d.attackSpeedMultiple >= 1f;
            AddLine($"AttackSpeed x{d.attackSpeedMultiple:0.##}", pos);
        }

        if (d.specialDamageMultiple != 0f)
        {
            bool pos = d.specialDamageMultiple >= 1f;
            AddLine($"SpecialDamage x{d.specialDamageMultiple:0.##}", pos);
        }
    }
}
