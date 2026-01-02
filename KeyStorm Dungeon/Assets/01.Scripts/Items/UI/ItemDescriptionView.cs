using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDescriptionView : MonoBehaviour
{

    [Header("UI Refs")]
    public Image frameImage;
    public Image iconImage;
    public TMP_Text nameText;
    public Transform statLinesRoot;
    public TMP_Text statLinePrefab;
    public TMP_Text extraDescText;
    public Transform tierEffectRoot;

    [Header("Multiplier 표시 방식")]
    [Tooltip("true = 1.2처럼 최종 배수로 저장 / false = 0.2처럼 증가량(20%)로 저장")]
    public bool multiplierStoredAsFinal = true;

    private readonly List<TMP_Text> _spawned = new();
    private GameObject _tierFx;

    public void SetItem(ItemData data)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (nameText) nameText.text = data.itemName;
        if (iconImage) iconImage.sprite = data.iconSprite;

        ClearStatLines();
        AddStatLine("MaxHp", data.maxHp);
        AddStatLine("MoveSpeed", data.moveSpeed);
        AddStatLine("Damage", data.damage);
        AddStatLine("SpecialDamage", data.specialDamageMultiple, isMultiplier: true);
        AddStatLine("Damage", data.damageMultiple, isMultiplier: true);
        AddStatLine("AttackSpeed", data.attackSpeed);
        AddStatLine("AttackSpeed", data.attackSpeedMultiple, isMultiplier: true);
        AddStatLine("Range", data.range);
        AddStatLine("ShotSpeed", data.shotSpeed);
        AddStatLine("MaxAmmo", data.maxAmmo);
        AddStatLine("UseAmmo", data.useAmmo);
        AddStatLine("Scale", data.scale);

        if (extraDescText)
        {
            extraDescText.text = data.description;
        }
    }

   

    private void ClearStatLines()
    {
        foreach (var t in _spawned)
            if (t != null) Destroy(t.gameObject);
        _spawned.Clear();
    }

    private void AddStatLine(string label, int value, bool isMultiplier = false)
    {
        if (value == 0) return;
        SpawnLine(label, value, isMultiplier);
    }

    private void AddStatLine(string label, float value, bool isMultiplier = false)
    {
        if (Mathf.Approximately(value, 0f)) return;
        SpawnLine(label, value, isMultiplier);
    }

    private void SpawnLine(string label, float value, bool isMultiplier)
    {
        if (statLinePrefab == null || statLinesRoot == null) return;

        var line = Instantiate(statLinePrefab, statLinesRoot);
        _spawned.Add(line);

        string sign = value > 0 ? "+" : "";
        string textValue;

        if (isMultiplier)
        {
            float shown = multiplierStoredAsFinal ? value : (1f + value);
            textValue = $"x{shown:0.##}";
        }
        else
        {
            textValue = $"{sign}{value:0.##}";
        }

        line.text = $"{label} {textValue}";

        line.color = value >= 0 ? Color.green : Color.red;
    }
}
