using UnityEngine;
using static ConstValue;

public class SpitterDieState : MonsterDieState
{
    public SpitterDieState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
    }

    public override void EnterState()
    {
        Collider2D monsterCollider = character.GetComponent<Collider2D>();
        if (monsterCollider != null)
        {
            monsterCollider.enabled = false;
        }

        if (ItemDropManager.Instance != null && character.MonsterData != null)
        {
            ItemDropManager.Instance.DropItems(character.transform.position, character.MonsterData);
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
}
