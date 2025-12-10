using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : CharacterDieState<Monster>
{
    public MonsterDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        if (character.MonsterRb != null)
        {
            character.MonsterRb.velocity = Vector2.zero;
            character.MonsterRb.simulated = false;
        }
        
        Collider2D monsterCollider = character.GetComponent<Collider2D>();
        if(monsterCollider != null)
        {
            monsterCollider.enabled = false;
        }

        if (character.Animator != null)
        {
            character.Animator.SetBool("IsDie", true);
            character.StartCoroutine(WaitForDieAnimation());
        }
        else
        {
            Object.Destroy(character.gameObject);
        }
    }

    IEnumerator WaitForDieAnimation()
    {

        yield return null;

        AnimatorStateInfo stateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);

        // 풀링 시 Destroy대신 ReturnPool 등 메서드 사용
        Object.Destroy(character.gameObject);
    }
}
