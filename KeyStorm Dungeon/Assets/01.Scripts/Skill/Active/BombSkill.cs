using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSkill : IActiveSKill
{
    BombData data;
    public Player Player { get; private set; }

    public bool IsFinish { get; private set; }

    public BombSkill(PlayerSkill playerSkill, BombData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }


    public void Enter()
    {
        Player.PlayerAttack.ActiveItemBomb();
        IsFinish = true;
    }


    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
