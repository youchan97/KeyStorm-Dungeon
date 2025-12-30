using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
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
            currentSkill.skill.Exit();
            currentSkill.cooldown = currentSkill.baseCooldown;
            currentSkill = null;
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
                cooldown = 0
            });
        }
    }

    void ApplyItemData()
    {
        foreach (var item in activeItems)
        {
            if (!skillDic.TryGetValue(item.skillType, out var skill))
                continue;

            skill.baseCooldown = item.cooldownMax;
            skill.cooldownType = item.cooldownType;
        }
    }


    public bool TrySkill(SkillType type)
    {
        if (!skillDic.TryGetValue(type, out var skill)) return false;
        if (skill.cooldown > 0) return false;

        currentSkill = skill;
        skill.skill.Enter();
        return true;
    }

    void UpdateCooldowns()
    {
        foreach (var skill in skillDic.Values)
        {
            if (skill.cooldown <= 0) continue;
            if (skill.cooldownType == ActiveCooldownType.PerTime)
                skill.cooldown -= Time.deltaTime;
        }
    }

    public void OnRoomClear()
    {
        foreach (var skill in skillDic.Values)
            if (skill.cooldownType == ActiveCooldownType.PerRoom)
                skill.cooldown = Mathf.Max(0, skill.cooldown - 1);
    }
}

public class RuntimeSkill
{
    public SkillType skillType;
    public ISkill skill;

    public float baseCooldown;
    public ActiveCooldownType cooldownType;

    public float cooldown;
}
