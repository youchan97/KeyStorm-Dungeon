using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class MeteoSkill : ISkill
{
    MeteoData data;
    Transform target;
    public Player Player { get; private set;}

    public bool IsFinish { get; private set;}

    public MeteoSkill(PlayerSkill playerSkill, MeteoData data)
    {
        Player = playerSkill.player;
        this.data = data;
    }

    public void Enter()
    {
        IsFinish = false;

        DetectMonster();

        if (target == null)
        {
            IsFinish = true;
            return;
        }

        Meteo meteo = Object.Instantiate(data.meteo);
        meteo.transform.position = target.position;
        meteo.InitData(Player, data);

        IsFinish = true;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }

    void DetectMonster()
    {
        Vector2 vec = new Vector2(data.boxSize, data.boxSize);
        Collider2D[] cols = Physics2D.OverlapBoxAll(Player.transform.position, vec, DefaultZero, data.enemyLayer);

        if (cols == null || cols.Length == DefaultIntZero)
            return;

        Monster monster = cols[Random.Range(DefaultIntZero, cols.Length)].GetComponent<Monster>();

        if (monster == null)
            return;

        target = monster.transform;
    }
}
