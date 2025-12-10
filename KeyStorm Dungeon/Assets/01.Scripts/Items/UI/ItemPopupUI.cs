using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupUI : MonoBehaviour
{
    public static ItemPopupUI Instance;

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Image iconImage; // 아이콘 쓸 거면

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(ItemData data)
    {
        nameText.text = data.itemName;
        descText.text = BuildDescriptionText(data);
        // iconImage.sprite = data.icon; // 나중에 아이콘 필드 추가

        StartCoroutine(ShowRoutine());
    }

    string BuildDescriptionText(ItemData data)
    {
        StringBuilder sb = new StringBuilder();

        if (data.maxHp != 0)
            sb.AppendLine($"HP {(data.maxHp > 0 ? "+" : "")}{data.maxHp}");
        if (data.moveSpeed != 0)
            sb.AppendLine($"이동속도 {(data.moveSpeed > 0 ? "+" : "")}{data.moveSpeed}");
        if (data.damage != 0)
            sb.AppendLine($"공격력 {(data.damage > 0 ? "+" : "")}{data.damage}");

        if (!string.IsNullOrEmpty(data.description))
        {
            sb.AppendLine();
            sb.AppendLine(data.description);
        }

        return sb.ToString();
    }

    IEnumerator ShowRoutine()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
    }
}
