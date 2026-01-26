using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class LookMapSkill : IActiveSKill
{
    LookMapData data;

    float timer;
    public Player Player { get; private set; }
    public bool IsFinish { get; private set; }


    public LookMapSkill(PlayerSkill playerSkill, LookMapData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }

    public void Enter()
    {
        IsFinish = false;
        Player.GameSceneUI.SetMinimap(true);
        timer = DefaultZero;
    }


    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= data.duration)
        {
            timer = DefaultZero;
            IsFinish = true;
        }
    }
    public void Exit()
    {
        Player.GameSceneUI.SetMinimap(false);
    }
}
