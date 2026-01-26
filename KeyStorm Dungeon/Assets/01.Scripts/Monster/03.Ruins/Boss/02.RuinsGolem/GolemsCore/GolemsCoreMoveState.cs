using System.Collections;
using UnityEngine;

public class GolemsCoreMoveState : MonsterMoveState
{
    private GolemsCore golemsCore;
    private Coroutine coroutine;
    private float currentAngel;
    private float distanceToGolem;
    private int rotationDirection;
    private float moveDuration;

    public GolemsCoreMoveState(Monster monster, CharacterStateManager<Monster> stateManager) : base(monster, stateManager)
    {
        golemsCore = character as GolemsCore;
    }

    public override void EnterState()
    {
        Vector2 localPosition = golemsCore.transform.localPosition;
        currentAngel = Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Deg2Rad;
        distanceToGolem = localPosition.magnitude;

        moveDuration = Random.Range(golemsCore.MinMoveDuration, golemsCore.MaxMoveDuration);
        rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1;

        coroutine = golemsCore.StartCoroutine(MoveCoroutine());
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        if (coroutine != null)
        {
            golemsCore.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public override bool UseFixedUpdate()
    {
        return false;
    }

    private IEnumerator MoveCoroutine()
    {
        float timer = 0f;

        while(timer < moveDuration)
        {
            timer += Time.deltaTime;
            currentAngel += rotationDirection * golemsCore.MoveSpeed * Time.deltaTime;

            float angleRad = currentAngel * Mathf.Rad2Deg;

            Vector2 newLocalPosition = new Vector2(Mathf.Cos(angleRad) * distanceToGolem, Mathf.Sin(angleRad) * distanceToGolem);
            golemsCore.transform.localPosition = newLocalPosition;

            yield return null;
        }

        stateManager.ChangeState(golemsCore.CreateIdleState());
    }
}
