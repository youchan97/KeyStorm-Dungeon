using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RuinsGolemPattern
{
    Slam,
    RockFall
}

public class RuinsGolemAttackState : MonsterAttackState
{
    private RuinsGolem ruinsGolem;
    private Coroutine attackCoroutine;

    private bool isSlamAnimationFinished;
    private bool isRockFallAnimationFinished;

    #region 애니메이션
    private const string SlamAnim = "Slam";
    private const string RockFallAnim = "RockFall";
    private const string BreakAnim = "Break";
    #endregion

    public RuinsGolemAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        ruinsGolem = character as RuinsGolem;
    }

    public override void EnterState()
    {
        base.EnterState();
        isSlamAnimationFinished = false;
        isRockFallAnimationFinished = false;

        ruinsGolem.OnSlamAnimation += HandleSlamAnimationFinished;
        ruinsGolem.OnRockFallAnimation += HandleRockFallAnimationFinished;

        attackCoroutine = ruinsGolem.StartCoroutine(AttackCoroutine());
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        if (attackCoroutine != null)
        {
            ruinsGolem.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        ruinsGolem.OnSlamAnimation -= HandleSlamAnimationFinished;
        ruinsGolem.OnRockFallAnimation -= HandleRockFallAnimationFinished;
    }

    public override bool UseFixedUpdate()
    {
        return base.UseFixedUpdate();
    }

    private IEnumerator AttackCoroutine()
    {
        RuinsGolemPattern selectedPattern = SelectPattern();

        switch(selectedPattern)
        {
            case RuinsGolemPattern.Slam:
                yield return SlamPattern();
                break;
            case RuinsGolemPattern.RockFall:
                yield return RockFallPattern();
                break;
        }

        yield return new WaitForSeconds(ruinsGolem.AttackDelay);

        stateManager.ChangeState(ruinsGolem.CreateIdleState());
    }

    private RuinsGolemPattern SelectPattern()
    {
        List<RuinsGolemPattern> patterns = new List<RuinsGolemPattern>();

        patterns.Add(RuinsGolemPattern.Slam);
        patterns.Add(RuinsGolemPattern.RockFall);

        int randomIndex = Random.Range(0, patterns.Count);
        return patterns[randomIndex];
    }

    private IEnumerator SlamPattern()
    {
        // 땅을 치는 횟수 3번
        for (int i = 0; i < 3; i++)
        {
            ruinsGolem.Animator.SetTrigger(SlamAnim);
            yield return new WaitUntil(() => isSlamAnimationFinished == true);
            ShakeCameraEvent.StartShakeCamera(ruinsGolem.ShakePower,ruinsGolem.ShakeDuration);

            float currentOuterRadius = ruinsGolem.SlamOuterRadius[i];
            float currentInnerRadius = ruinsGolem.SlamInnerRadius[i];

            GameObject slamEffectGO = GameObject.Instantiate(ruinsGolem.SlamEffect, ruinsGolem.transform.position, Quaternion.identity);
            slamEffectGO.transform.localScale = Vector3.one * currentOuterRadius * (i);

            Vector2 checkPosition = (Vector2)ruinsGolem.transform.position;
            Collider2D[] hitCollidersOuter = Physics2D.OverlapCircleAll(checkPosition, currentOuterRadius, ruinsGolem.PlayerLayer);
            Collider2D[] hitCollidersInner = Physics2D.OverlapCircleAll(checkPosition, currentInnerRadius, ruinsGolem.PlayerLayer);

            List<Player> hitPlayers = new List<Player>();

            foreach (Collider2D hitOuter in hitCollidersOuter)
            {
                if (((1 << hitOuter.gameObject.layer) & ruinsGolem.PlayerLayer.value) > 0)
                {
                    Player player = hitOuter.GetComponent<Player>();
                    if (player != null && !hitPlayers.Contains(player))
                    {
                        hitPlayers.Add(player);
                    }
                }
            }

            foreach (Collider2D hitInner in hitCollidersInner)
            {
                if (((1 << hitInner.gameObject.layer) & ruinsGolem.PlayerLayer.value) > 0)
                {
                    Player player = hitInner.GetComponent<Player>();
                    if (player != null)
                    {
                        hitPlayers.Remove(player);
                    }
                }
            }

            foreach (Player player in hitPlayers)
            {
                player.TakeDamage(ruinsGolem.Damage);
            }

            if ( i < 2)
            {
                yield return new WaitForSeconds(ruinsGolem.SlamDelay);
            }

            isSlamAnimationFinished = false;
        }
    }

    private IEnumerator RockFallPattern()
    {
        ruinsGolem.Animator.SetTrigger(RockFallAnim);
        yield return new WaitUntil(() => isRockFallAnimationFinished == true);
        ShakeCameraEvent.StartShakeCamera(ruinsGolem.RockFallShakeCameraPower, ruinsGolem.RockFallShakeCameraDuration);

        float timer = 0f;

        while (timer < ruinsGolem.AllRockFallDuration)
        {
            float addTime = Random.Range(ruinsGolem.MinRockSpawnTime, ruinsGolem.MaxRockSpawnTime);
            timer += addTime;
            yield return new WaitForSeconds(addTime);

            if (timer >= ruinsGolem.AllRockFallDuration) break;

            Vector2 spawnRockPosition = ruinsGolem.GetRandomTilemapInRoom();

            ruinsGolem.StartCoroutine(RockFall(spawnRockPosition, ruinsGolem.ShadowDuration, ruinsGolem.ShadowPrefab, ruinsGolem.RockPrefab, ruinsGolem.Damage, ruinsGolem.PlayerLayer));
        }
    }

    private IEnumerator RockFall(Vector3 dropPosition, float shadowDuration, GameObject shadowPrefab, GameObject rockPrefab, float damage, LayerMask playerLayer)
    {
        GameObject shadowGO = GameObject.Instantiate(shadowPrefab, dropPosition, Quaternion.identity);
        SpriteRenderer shadowSpriteRenderer = shadowGO.GetComponent<SpriteRenderer>();

        float timer = 0f;

        shadowGO.transform.localScale = Vector3.one * ruinsGolem.MinShadowScale;
        while(timer < shadowDuration)
        {
            timer += Time.deltaTime;

            float currentScale = Mathf.Lerp(ruinsGolem.MinShadowScale, ruinsGolem.MaxShadowScale, timer / shadowDuration);
            shadowGO.transform.localScale = Vector3.one * currentScale;

            yield return null;
        }

        Vector3 rockSpawnPosition = dropPosition + Vector3.up * ruinsGolem.RockSpawnHeight;
        GameObject rockGO = GameObject.Instantiate(rockPrefab, rockSpawnPosition, Quaternion.identity);
        Animator rockAnimator = rockGO.GetComponent<Animator>();

        timer = 0f;
        Vector3 startFallPosition = rockGO.transform.position;
        while(timer < ruinsGolem.SingleRockFallDuration)
        {
            timer += Time.deltaTime;
            rockGO.transform.position = Vector3.Lerp(startFallPosition, dropPosition, timer / ruinsGolem.SingleRockFallDuration);
            yield return null;
        }
        rockGO.transform.position = dropPosition;

        GameObject.Destroy(shadowGO);
        rockAnimator.SetTrigger(BreakAnim);

        float damageRadius = ruinsGolem.DamageRadius;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(dropPosition, damageRadius, playerLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (((1 << hitCollider.gameObject.layer) & playerLayer.value) > 0)
            {
                Player hitPlayer = hitCollider.GetComponent<Player>();
                if (hitPlayer != null)
                {
                    hitPlayer.TakeDamage(damage);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(rockGO);
    }

    private void HandleSlamAnimationFinished()
    {
        isSlamAnimationFinished = true;
    }

    private void HandleRockFallAnimationFinished()
    {
        isRockFallAnimationFinished = true;
    }
}
