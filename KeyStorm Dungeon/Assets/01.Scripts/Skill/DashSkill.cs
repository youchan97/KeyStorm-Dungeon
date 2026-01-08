using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class DashSkill : ISkill
{
    DashData data;
    Vector2 dir;
    float timer;
    Rigidbody2D rb;
    PlayerController controller;
    public Player Player { get; private set; }

    public bool IsFinish { get; private set; }

    public DashSkill(PlayerSkill playerSkill, DashData data)
    {
        Player = playerSkill.player;
        controller = Player.PlayerController;
        this.data = data;
    }

    public void Enter()
    {
        IsFinish = false;
        timer = 0f;
        rb = Player.PlayerRb;
        dir = controller.MoveVec.normalized;
        if (dir == Vector2.zero)
        {
            dir = (controller.LastVec == Vector2.zero) ? Vector2.down : controller.LastVec.normalized;
        }
        Player.AudioManager.PlayEffect(DashSfx);
        Player.IsDashing = true;
    }
    public void Update()
    {
        timer += Time.deltaTime;
        rb.velocity = dir * data.dashSpeed;

        if (timer >= data.duration)
            IsFinish = true;
    }

    public void Exit()
    {
        rb.velocity = Vector2.zero;
        Player.IsDashing = false;
    }

}
