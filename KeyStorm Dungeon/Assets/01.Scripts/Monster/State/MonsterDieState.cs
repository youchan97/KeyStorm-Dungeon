using System.Collections;
using UnityEngine;
using static ConstValue;

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

        if (character.ItemDropSwitch)
        {
            if (ItemDropManager.Instance != null && character.MonsterData != null)
            {
                ItemDropManager.Instance.DropItems(character.transform.position, character.MonsterData);
            }
        }

        if (character.Animator != null)
        {
            character.Animator.SetTrigger(DieAnim);
            character.StartCoroutine(WaitForDieAnimation());
        }
        else
        {
            OnDeathDestroy();
        }
    }

    protected IEnumerator WaitForDieAnimation()
    {
        yield return null;

        AnimatorStateInfo stateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);
        
        OnDeathDestroy();
    }

    protected void OnDeathDestroy()
    {
        character.InvokeOnMonsterDied();

        Object.Destroy(character.gameObject);
    }
}
