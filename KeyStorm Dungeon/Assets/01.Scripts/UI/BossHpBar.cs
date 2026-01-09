using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private Image hpBar;

    private Monster monster;

    public event Action<BossHpBar> OnRemoveUI;

    private void OnDisable()
    {
        if (monster != null)
        {
            monster.OnTakeDamage -= UpdateHpBar;
            monster.OnMonsterDied -= OnBossDied;
            monster = null;
        }
    }

    public void InitBossInfo(Monster boss)
    {
        monster = boss;

        if (monster != null)
        {
            bossName.text = monster.MonsterData.characterData.charName;
            hpBar.fillAmount = monster.Hp / monster.MaxHp;
        }

        monster.OnTakeDamage += UpdateHpBar;
        monster.OnMonsterDied += OnBossDied;
    }

    private void UpdateHpBar()
    {
        if (monster != null)
        {
            hpBar.fillAmount = monster.Hp / monster.MaxHp;
        }
    }

    private void OnBossDied()
    {
        if (monster != null)
        {
            monster.OnTakeDamage -= UpdateHpBar;
            monster.OnMonsterDied -= OnBossDied;
            monster = null;
        }
        OnRemoveUI?.Invoke(this);
    }
}
