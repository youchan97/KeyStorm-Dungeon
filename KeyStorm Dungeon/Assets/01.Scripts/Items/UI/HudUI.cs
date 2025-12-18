using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public static HudUI Instance { get; private set; }

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI bombText;
    public Image activeItemIcon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateGold(int gold)
    {
        if (goldText != null) goldText.text = gold.ToString();
    }

    public void UpdateBomb(int bombs)
    {
        if (bombText != null) bombText.text = bombs.ToString();
    }

    public void SetActiveItem(ItemData data)
    {
        if (activeItemIcon == null) return;

        if (data == null || data.iconSprite == null)
        {
            activeItemIcon.sprite = null;
            activeItemIcon.enabled = false;
            return;
        }

        activeItemIcon.sprite = data.iconSprite;
        activeItemIcon.enabled = true;
    }
}
