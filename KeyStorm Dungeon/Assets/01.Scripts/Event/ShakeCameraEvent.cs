using System;

public static class ShakeCameraEvent
{
    /// <summary>
    /// shakePower(흔들림 강도), shakeDuration (흔들림 지속 시간)
    /// </summary>
    public static event Action<float, float> OnShakeCamera;

    public static void StartShakeCamera(float shakePower, float shakeDuration)
    {
        OnShakeCamera?.Invoke(shakePower, shakeDuration);
    }
}
