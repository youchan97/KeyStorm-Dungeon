using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : IPassiveSkill
{
    public Player Player { get; private set; }

    protected PassiveSkillData skillData;
    protected float interval;
    protected float timer;

    protected PassiveSkill(PlayerSkill playerSkill, PassiveSkillData data)
    {
        Player = playerSkill.player;
        skillData = data;
        this.interval = data.interval;
        timer = data.interval;
    }


    public void Durate(float time)
    {
        timer -= time;
        if (timer > 0f)
            return;

        Activate();
        timer = interval;
    }

    public virtual void DoPassive(float time) { }

    protected abstract void Activate();
}
