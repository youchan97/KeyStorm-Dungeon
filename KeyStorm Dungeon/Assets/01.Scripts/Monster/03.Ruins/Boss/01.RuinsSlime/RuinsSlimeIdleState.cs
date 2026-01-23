using UnityEngine;

public class RuinsSlimeIdleState : SlimeIdleState
{
    protected RuinsSlime ruinsSlime;

    public RuinsSlimeIdleState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        ruinsSlime = character as RuinsSlime;
    }

    public override void EnterState()
    {
        if (ruinsSlime.MonsterRb != null)
        {
            ruinsSlime.MonsterRb.velocity = Vector2.zero;
        }

        if (ruinsSlime.PlayerGO == null) return;

        float randomIdleTime = Random.Range(ruinsSlime.MinIdleTime, ruinsSlime.MaxIdleTime);
        currentIdleTime = randomIdleTime;
    }

}
