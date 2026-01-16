using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class PlayerDotweenManager : MonoBehaviour
{
    #region readOnlyValue
    private readonly Vector3 oneCycleRotate = new Vector3(0f, 0f, 360f);
    #endregion

    [SerializeField] Player player;

    [Header("Portal")]
    Sequence portalSequence;
    [SerializeField] float jumpPower;
    [SerializeField] float duration;
    [SerializeField] GameObject playerUi;

    Sequence CreateSequence(Sequence sequence, bool isAutoKill = false)
    {
        sequence = DOTween.Sequence();
        sequence.SetAutoKill(isAutoKill);

        return sequence;
    }


    void ResetSequence(ref Sequence sequence)
    {
        if(sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }
    }

    public void GameOverCanvas()
    {
        player.GameOverCanvas();
    }

    public void PortalDotween(Vector3 portalPos, Player player)
    {
        player.PlayerController.DisableInput();
        playerUi.SetActive(false);
        ResetSequence(ref portalSequence);
        portalSequence = CreateSequence(portalSequence);

        portalSequence.Append(transform.DOJump(portalPos, jumpPower, DefaultJumpCount, duration).SetEase(Ease.InQuad))
            .Append(transform.DORotate(oneCycleRotate, duration, RotateMode.FastBeyond360))
            .Join(transform.DOScale(Vector3.zero, duration))
            .OnComplete(() => { GameManager.Instance.StageClear(); });
    }
}
