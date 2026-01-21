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
        IsFinish = false;
        GameObject go = Object.Instantiate(data.dynamite);
        go.transform.position = Player.transform.position;
        Dynamite dynamite = go.GetComponent<Dynamite>();

        if (dynamite == null)
            return;

        dynamite.InitData(data, Player);
        IsFinish = true;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}
