using System;
using UnityEngine;
using System.Collections;

public class BossMonster : Monster
{
    [Header("보스 패턴 수치 설정")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float diveDelay;
    [SerializeField] private float diveDuration;
    [SerializeField] private GameObject bossShadow;
    [SerializeField] private float shadowScaleTime;
    [SerializeField] private float minShadowScale = 0.1f;
    [SerializeField] private float maxShadowScale = 1.0f;
    [SerializeField] private float landedDelay;

    public GameObject BossShadow => bossShadow;

    private MonsterIdleState _idleState;
    private BossMonsterMoveState _moveState;
    private BossMonsterAttackState _attackState;
    private MonsterDieState _dieState;

    public Action OnJumpAnimationFinished;

    public void OnJumpAnimationEnd()
    {
        OnJumpAnimationFinished?.Invoke();
    }
    
    public override CharacterState<Monster> CreateIdleState()
    {
        if (_idleState == null) _idleState = new MonsterIdleState(this, MonsterStateManager);
        return _idleState;
    }

    public override CharacterState<Monster> CreateMoveState()
    {
        if (_moveState == null) _moveState = new BossMonsterMoveState(this, MonsterStateManager);
        return _moveState;
    }

    public override CharacterState<Monster> CreateAttackState()
    {
        if (_attackState == null) _attackState = new BossMonsterAttackState(this, MonsterStateManager);
        return _attackState;
    }

    public override CharacterState<Monster> CreateDieState()
    {
        if (_dieState == null) _dieState = new MonsterDieState(this, MonsterStateManager);
        return _dieState;
    }

    protected override void Start()
    {
        if (bossShadow != null)
        {
            bossShadow.SetActive(false);
        }

        base.Start();
    }

    public IEnumerator DiveAttackCoroutine(Vector3 targetDivePosition)
    {
        Debug.Log("DiveAttackCoroutine 실행됨");
        Animator.SetTrigger("IsJump");

        bool jumpAnimFinished = false;
        Action jumpFinishedAction = () => jumpAnimFinished = true;
        OnJumpAnimationFinished += jumpFinishedAction;

        yield return new WaitUntil(() => jumpAnimFinished == true);
        OnJumpAnimationFinished -= jumpFinishedAction;

        Vector3 initialBossPos = transform.position;
        Vector3 peakJumpPos = initialBossPos + Vector3.up * jumpHeight;

        float timer = 0f;
        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialBossPos, peakJumpPos, timer / jumpDuration);
            yield return null;
        }

        transform.position = peakJumpPos;
        GetComponent<Collider2D>().enabled = false;

        if (bossShadow != null)
        {
            bossShadow.SetActive(true);
            bossShadow.transform.localScale = Vector3.one * minShadowScale;

            float shadowTimer = 0f;
            while (shadowTimer < shadowScaleTime)
            {
                shadowTimer += Time.deltaTime;
                float currentScale = Mathf.Lerp(minShadowScale, maxShadowScale, shadowTimer / shadowScaleTime);
                bossShadow.transform.localScale = Vector3.one * currentScale;
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("BossMonsterAttackState: ShadowPrefab이 할당되지 않음.");
        }

        yield return new WaitForSeconds(diveDelay - shadowScaleTime);

        if (bossShadow != null)
        {
            bossShadow.SetActive(false);
        }

        transform.position = targetDivePosition + Vector3.up * jumpHeight * 0.5f;
        GetComponent<Collider2D>().enabled = true;

        Animator.SetTrigger("IsDive");

        Vector3 startDivePos = transform.position;
        Vector3 endDivePos = targetDivePosition;

        timer = 0f;
        while (timer < diveDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startDivePos, endDivePos, timer / diveDuration);
            yield return null;
        }

        transform.position = endDivePos;

        ApplyLandingDamage(endDivePos, maxShadowScale);

        yield return new WaitForSeconds(landedDelay);

        Debug.Log("다이브어택코루틴끝남");
    }

    private void ApplyLandingDamage(Vector3 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(Damage);
            }
        }
    }
}
