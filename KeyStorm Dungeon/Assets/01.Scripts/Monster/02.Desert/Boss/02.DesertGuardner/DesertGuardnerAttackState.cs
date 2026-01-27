using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public enum DesertGuardnerPattern
{
    Spin,
    Swing,
    Reflect
}

public class DesertGuardnerAttackState : MonsterAttackState
{
    private DesertGuardner desertGuardner;
    private Coroutine attackCoroutine;

    private float currentSpinAngle;
    private float bulletTime;

    private bool isTakeShieldAnimationFinished;
    private bool isTakeOffShieldAnimationFinished;

    #region 애니메이션
    private const string SpinAttackAnim = "SpinAttack";
    private const string SwingAttackAnim = "SwingAttack";
    private const string TakeShieldAnim = "TakeShield";
    private const string ShieldingAnim = "Shielding";
    #endregion


    public DesertGuardnerAttackState(Monster character, CharacterStateManager<Monster> stateManager) : base(character, stateManager)
    {
        desertGuardner = character as DesertGuardner;
    }

    public override void EnterState()
    {
        base.EnterState();
        desertGuardner.StopSpin();

        desertGuardner.ResetFilpSprite();

        isTakeShieldAnimationFinished = false;
        isTakeOffShieldAnimationFinished = false;

        desertGuardner.OnTakeShieldAnimation += HandleTakeShieldAnimationFinished;
        desertGuardner.OnTakeOffShieldAnimation += HandleTakeOffShieldAnimationFinished;

        attackCoroutine = desertGuardner.StartCoroutine(AttackCoroutine());
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        if (desertGuardner.PlayerGO == null)
        {
            desertGuardner.ChangeStateToPlayerDied();
            return;
        }

        if (desertGuardner.IsSpin)
        {
            Vector2 direction = (desertGuardner.PlayerTransform.position - desertGuardner.transform.position).normalized;
            animator.SetFloat(AxisX, direction.x);
            rb.velocity = direction * desertGuardner.SpinPatternMoveSpeed;
        }
    }

    public override void ExitState()
    {
        if (attackCoroutine != null)
        {
            desertGuardner.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        desertGuardner.OnTakeShieldAnimation -= HandleTakeShieldAnimationFinished;
        desertGuardner.OnTakeOffShieldAnimation -= HandleTakeOffShieldAnimationFinished;

        if (desertGuardner.IsSpin)
        {
            desertGuardner.StopSpin();
        }
        else if (desertGuardner.IsReflect)
        {
            desertGuardner.StopReflect();
        }
    }

    public override bool UseFixedUpdate()
    {
        return true;
    }

    private IEnumerator AttackCoroutine()
    {
        DesertGuardnerPattern selectedPattern = SelectPattern();

        switch (selectedPattern)
        {
            case DesertGuardnerPattern.Spin:
                yield return SpinPattern();
                break;
            case DesertGuardnerPattern.Swing:
                yield return SwingPattern();
                break;
            case DesertGuardnerPattern.Reflect:
                yield return ReflectPattern();
                break;
        }

        stateManager.ChangeState(desertGuardner.CreateIdleState());
    }

    private DesertGuardnerPattern SelectPattern()
    {
        List<DesertGuardnerPattern> patterns = new List<DesertGuardnerPattern>();

        patterns.Add(DesertGuardnerPattern.Spin);
        patterns.Add(DesertGuardnerPattern.Swing);

        if(desertGuardner.CurrentReflectPatternCooldown <= 0f)
        {
            patterns.Add(DesertGuardnerPattern.Reflect);
        }

        int randomIndex = Random.Range(0, patterns.Count);
        return patterns[randomIndex];
    }

    private IEnumerator SpinPattern()
    {
        animator.SetBool(SpinAttackAnim, true);

        desertGuardner.StartSpin();

        float currentTime = 0f;
        
        while (currentTime < desertGuardner.SpinPatternDuration)
        {
            currentTime += Time.deltaTime;
            bulletTime += Time.deltaTime;

            currentSpinAngle += desertGuardner.SpinRotationSpeed * Time.deltaTime;
            currentSpinAngle %= 360f;

            if(bulletTime >= desertGuardner.BulletInterval)
            {
                bulletTime -= desertGuardner.BulletInterval;

                Vector2 currentMoveDirection = (desertGuardner.PlayerTransform.position - desertGuardner.transform.position).normalized;

                float angleRad = currentSpinAngle * Mathf.Deg2Rad;
                float angleX = Mathf.Cos(angleRad);
                if (currentMoveDirection.x > 0f)
                {
                    angleX *= -1f;
                }

                Vector3 spinAttackPoint = desertGuardner.transform.position + new Vector3(angleX, Mathf.Sin(angleRad), 0) * desertGuardner.SpinRadius;

                Vector2 bulletDirection = (spinAttackPoint - desertGuardner.transform.position).normalized;

                desertGuardner.FireBullet(bulletDirection);
            }
            yield return null;
        }

        desertGuardner.StopSpin();
        animator.SetBool(SpinAttackAnim, false);
    }

    private IEnumerator SwingPattern()
    {
        Vector2 direction = (desertGuardner.PlayerTransform.position - desertGuardner.transform.position).normalized;

        desertGuardner.FlipSprite(direction.x);
        animator.SetTrigger(SwingAttackAnim);

        yield return new WaitForSeconds(desertGuardner.SwingDelay);
    }

    private IEnumerator ReflectPattern()
    {
        Vector2 direction = (desertGuardner.PlayerTransform.position - desertGuardner.transform.position).normalized;
        desertGuardner.FlipSprite(direction.x);

        animator.SetTrigger(TakeShieldAnim);

        yield return new WaitUntil(() => isTakeShieldAnimationFinished == true);

        animator.SetBool(ShieldingAnim, true);
        desertGuardner.StartReflect();

        yield return new WaitForSeconds(desertGuardner.ReflectPatternDuration);

        animator.SetBool(ShieldingAnim, false);
        yield return new WaitUntil(() => isTakeOffShieldAnimationFinished == true);

        desertGuardner.ResetReflectPatternCooldown();
    }

    private void HandleTakeShieldAnimationFinished()
    {
        isTakeShieldAnimationFinished = true;
    }

    private void HandleTakeOffShieldAnimationFinished()
    {
        isTakeOffShieldAnimationFinished = true;
    }
}
