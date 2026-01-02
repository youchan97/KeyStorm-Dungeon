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
    //[SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    private int maxHp;
    private int hp;

    public void SetMaxHp(int newMaxHp)
    {
        maxHp = Mathf.Max(0, newMaxHp);

        RebuildHearts(maxHp);

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
            hearts[i].sprite = (i < hp) ? fullHeart : emptyHeart;
        }
    }
}
