using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Player player;

    [SerializeField] CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakePower;

    private void Start()
    {
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDisable()
    {
        player.OnHit -= ShakeCamera;
    }

    public void SetTarget(Player player)
    {
        this.player = player;
        player.OnHit += ShakeCamera;
    }

    void ShakeCamera()
    {
        noise.m_AmplitudeGain = shakePower;

        DOTween.Kill(noise);
        DOTween.To(
            () => noise.m_AmplitudeGain,
            x => noise.m_AmplitudeGain = x,
            0f,
            shakeDuration
        ).SetTarget(noise);
    }
}
