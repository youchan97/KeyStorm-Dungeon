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

    private void OnEnable()
    {
        ShakeCameraEvent.OnShakeCamera += ShakeCamera;
    }
    private void OnDisable()
    {
        player.OnHit -= ShakeCameraHitPlayer;
        ShakeCameraEvent.OnShakeCamera -= ShakeCamera;
    }

    public void SetTarget(Player player)
    {
        this.player = player;
        player.OnHit += ShakeCameraHitPlayer;
    }

    private void ShakeCameraHitPlayer()
    {
        ShakeCamera(shakePower, shakeDuration);
    }

    public void ShakeCamera(float power, float duration)
    {
        noise.m_AmplitudeGain = power;

        DOTween.Kill(noise);
        DOTween.To(
            () => noise.m_AmplitudeGain,
            x => noise.m_AmplitudeGain = x,
            0f,
            duration
        ).SetTarget(noise);
    }
}
