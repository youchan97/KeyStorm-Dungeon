using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Image heartPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    private int maxHp;
    private int hp;

    private void Update()
    {
        SetMaxHp(GameManager.Instance.PlayerRunData.character.maxHp);
        SetHp(GameManager.Instance.PlayerRunData.character.currentHp);
    }

    public void SetMaxHp(int newMaxHp)
    {
        maxHp = Mathf.Max(0, newMaxHp);

        int needHeartCount = Mathf.CeilToInt(maxHp / 2f);
        RebuildHearts(needHeartCount);

        hp = Mathf.Clamp(hp, 0, maxHp);
        Refresh();
    }

    public void SetHp(int newHp)
    {
        hp = Mathf.Clamp(newHp, 0, maxHp);
        Refresh();
    }

    private void RebuildHearts(int count)
    {
        for (int i = hearts.Count - 1; i >= count; i--)
        {
            if (hearts[i] != null) Destroy(hearts[i].gameObject);
            hearts.RemoveAt(i);
        }

        while (hearts.Count < count)
        {
            Image img = Instantiate(heartPrefab, heartContainer);
            hearts.Add(img);
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStart = i * 2;
            int remaining = hp - heartStart;

            if (remaining >= 2) hearts[i].sprite = fullHeart;
            else if (remaining == 1) hearts[i].sprite = halfHeart;
            else hearts[i].sprite = emptyHeart;
        }
    }
}
