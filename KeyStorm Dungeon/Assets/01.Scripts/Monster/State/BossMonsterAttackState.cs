
using System;
using UnityEngine;

public class BossMonsterAttackState : MonsterAttackState
{
    private BossMonster boss;
    private Transform playerTransform;
    private Vector3 divePosition;
    private GameObject currentShadow;


    public BossMonsterAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        this.boss = character as BossMonster;
    }

    public override void EnterState()
    {
        Debug.Log("AttackState 진입");
        playerTransform = boss.PlayerTransform;

        if(boss.MonsterRb != null)
        {
            boss.MonsterRb.velocity = Vector2.zero;
        }

        if(playerTransform == null)
        {
            stateManager.ChangeState(boss.CreateIdleState());
            return;
        }

        if(boss.BossShadow != null)
        {
            currentShadow = boss.BossShadow;
        }

        divePosition = playerTransform.position;

        boss.StartCoroutine(boss.DiveAttackCoroutine(divePosition));
    }

    public override void ExitState()
    {
        boss.StopAllCoroutines();

        boss.GetComponent<Collider2D>().enabled = true;

        if(currentShadow != null)
        {
            currentShadow.SetActive(false);
        }

        boss.Animator.ResetTrigger("IsJump");
        boss.Animator.ResetTrigger("IsDive");
        Debug.Log("AttackState 퇴장");
    }
}
