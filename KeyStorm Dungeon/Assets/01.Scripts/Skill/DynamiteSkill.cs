using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteSkill : ISkill
{
    DynamiteData data;
    public Player Player { get; private set; }

    public bool IsFinish { get; private set; }

    public DynamiteSkill(PlayerSkill playerSkill, DynamiteData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
