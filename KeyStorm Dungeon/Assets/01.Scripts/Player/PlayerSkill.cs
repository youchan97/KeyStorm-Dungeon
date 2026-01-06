using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    Player Player { get; }
    void Enter();
    void Update();
    void Exit();
    bool IsFinish { get; }
}

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] SkillSet skillSet;

    [SerializeField] List<ItemData> activeItems;

    Dictionary<SkillType, RuntimeSkill> skillDic = new Dictionary<SkillType, RuntimeSkill>();
    RuntimeSkill currentSkill;
    List<RuntimeSkill> cooldownSkills = new List<RuntimeSkill>();

    public event Action<float> StartSkill;

    public Player player;

    void Awake()
    {
        InitSkills();
        ApplyItemData();
    }

    void Update()
    {
        UpdateCooldowns();

        if (currentSkill == null) return;

        currentSkill.skill.Update();

        if (currentSkill.skill.IsFinish)
        {
            FinishSkill();
        }
    }

    void InitSkills()
    {
        foreach (var skillData in skillSet.skills)
        {
            ISkill skill = skillData.CreateSkill(this);

            skillDic.Add(skillData.skillType, new RuntimeSkill
            {
                skill = skill,
                coolTime = 0
            });
        }
    }

    void ApplyItemData()
    {
        foreach (var item in activeItems)
        {
            if (!skillDic.TryGetValue(item.skillType, out var skill))
                continue;

            skill.baseCoolTime = item.cooldownMax;
            skill.cooldownType = item.cooldownType;
        }
    }


    public bool TrySkill(SkillType type)
    {
        if (!skillDic.TryGetValue(type, out var skill)) return false;
        if (skill.coolTime > 0 || currentSkill != null) return false;

        currentSkill = skill;
        skill.skill.Enter();
        return true;
    }

    void FinishSkill()
    {
        currentSkill.skill.Exit();
        currentSkill.coolTime = currentSkill.baseCoolTime;
        StartSkill?.Invoke(currentSkill.coolTime);
        if (currentSkill.cooldownType == ActiveCooldownType.PerTime)
            cooldownSkills.Add(currentSkill);
        currentSkill = null;
    }

    void UpdateCooldowns()
    {
        if (cooldownSkills.Count == 0)
            return;

        float time = Time.deltaTime;

        for (int i = cooldownSkills.Count - 1; i >= 0; i--)
        {
            RuntimeSkill skill = cooldownSkills[i];
            skill.coolTime -= time;

            if (skill.coolTime <= 0f)
            {
                skill.coolTime = 0f;
                cooldownSkills.RemoveAt(i);
            }
        }
    }

    public void OnRoomClear()
    {
        for (int i = cooldownSkills.Count - 1; i >= 0; i--)
        {
            var skill = cooldownSkills[i];

            if (skill.cooldownType != ActiveCooldownType.PerRoom)
                continue;

            skill.coolTime = Mathf.Max(0, skill.coolTime - 1);

            if (skill.coolTime <= 0)
                cooldownSkills.RemoveAt(i);
        }
    }
}

public class RuntimeSkill
{
    public SkillType skillType;
    public ISkill skill;

    public float baseCoolTime;
    public ActiveCooldownType cooldownType;

    public float coolTime;
}
