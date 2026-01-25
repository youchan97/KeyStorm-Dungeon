using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    Player Player { get; }
}

public interface IActiveSKill : ISkill
{
    void Enter();
    void Update();
    void Exit();
    bool IsFinish { get; }
}

public interface IPassiveSkill : ISkill
{
    void Durate(float time);
    void DoPassive(float time);
}


public class PlayerSkill : MonoBehaviour
{
    [SerializeField] SkillSet skillSet;

    [SerializeField] List<ItemData> activeItems;

    Dictionary<SkillType, RuntimeActiveSkill> activeSkillDic = new Dictionary<SkillType, RuntimeActiveSkill>();
    List<RuntimeActiveSkill> activeSkills = new List<RuntimeActiveSkill>();
    RuntimeActiveSkill currentSkill;
    Dictionary<SkillType, RuntimePassiveSkill> passiveSkillDic = new Dictionary<SkillType, RuntimePassiveSkill>();
    List<RuntimePassiveSkill> passiveSkills = new List<RuntimePassiveSkill>();

    public event Action<float> StartSkill;

    public Player player;

    void Awake()
    {
        InitSkills();
        ApplyItemData();
        player.Inventory.OnAddPassiveItem += CheckPassiveSkill;
    }

    private void OnDisable()
    {
        player.Inventory.OnAddPassiveItem -= CheckPassiveSkill;
    }

    void Update()
    {
        float time = Time.deltaTime;

        UpdatePassiveSkills(time);
        UpdateActiveCooldowns(time);
        UpdateActiveSkill();
    }

    void InitSkills()
    {
        foreach (var skillData in skillSet.skills)
        {
            if (skillData is ActiveSkillData activeData)
            {
                IActiveSKill skill = activeData.CreateActiveSkill(this);

                activeSkillDic.Add(skillData.skillType, new RuntimeActiveSkill
                {
                    skillType = skillData.skillType,
                    skill = skill,
                    coolTime = 0f
                });
            }
        }
    }

    void CheckPassiveSkill(ItemData data)
    {
        if (passiveSkillDic.ContainsKey(data.skillType))
            return; // 이미 있음

        if (data.passiveSkillData == null)
            return;

        IPassiveSkill passive = data.passiveSkillData.CreatePassiveSkill(this);

        var runtime = new RuntimePassiveSkill
        {
            skill = passive
        };

        passiveSkillDic.Add(data.skillType, runtime);
        passiveSkills.Add(runtime);
    }

    void ApplyItemData()
    {
        foreach (var item in activeItems)
        {
            if (!activeSkillDic.TryGetValue(item.skillType, out var skill))
                continue;

            skill.baseCoolTime = item.cooldownMax;
            skill.cooldownType = item.cooldownType;
        }
    }

    void UpdateActiveSkill()
    {
        if (currentSkill == null) return;

        currentSkill.skill.Update();

        if (currentSkill.skill.IsFinish)
        {
            FinishActiveSkill();
        }
    }

    public bool TrySkill(SkillType type)
    {
        if (!activeSkillDic.TryGetValue(type, out var skill)) 
            return false;
        if (skill.coolTime > 0 || currentSkill != null)
            return false;

        currentSkill = skill;
        skill.skill.Enter();
        return true;
    }

    void FinishActiveSkill()
    {
        currentSkill.skill.Exit();
        currentSkill.coolTime = currentSkill.baseCoolTime;
        StartSkill?.Invoke(currentSkill.coolTime);
        if (currentSkill.cooldownType == ActiveCooldownType.PerTime)
            activeSkills.Add(currentSkill);
        currentSkill = null;
    }

    void UpdateActiveCooldowns(float time)
    {
        if (activeSkills.Count == 0)
            return;

        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            RuntimeActiveSkill skill = activeSkills[i];
            skill.coolTime -= time;

            if (skill.coolTime <= 0f)
            {
                skill.coolTime = 0f;
                activeSkills.RemoveAt(i);
            }
        }
    }

    void UpdatePassiveSkills(float time)
    {
        if (passiveSkills.Count == 0)
            return;

        for (int i = 0; i < passiveSkills.Count; i++)
        {
            passiveSkills[i].skill.Durate(time);
            passiveSkills[i].skill.DoPassive(time);
        }
    }

    public void OnRoomClear()
    {
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            RuntimeActiveSkill skill = activeSkills[i];

            if (skill.cooldownType != ActiveCooldownType.PerRoom)
                continue;

            skill.coolTime = Mathf.Max(0, skill.coolTime - 1);

            if (skill.coolTime <= 0)
                activeSkills.RemoveAt(i);
        }
    }
}

public class RuntimeActiveSkill
{
    public SkillType skillType;
    public IActiveSKill skill;

    public float baseCoolTime;
    public ActiveCooldownType cooldownType;

    public float coolTime;
}

public class RuntimePassiveSkill
{
    public IPassiveSkill skill;
}
