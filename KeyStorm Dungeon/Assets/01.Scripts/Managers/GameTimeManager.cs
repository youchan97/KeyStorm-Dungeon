using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    private float playTime = 0f;
    private bool isTimerRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameTimeManager] 생성됨");
        }
        else if (Instance != this)
        {
            Debug.Log("[GameTimeManager] 중복 제거");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            playTime += Time.unscaledDeltaTime;
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        Debug.Log("[GameTimeManager] 타이머 시작");
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
        Debug.Log("[GameTimeManager] 타이머 정지");
    }

    public void ResetTimer()
    {
        playTime = 0f;
        isTimerRunning = false;
        Debug.Log("[GameTimeManager] 타이머 리셋");
    }

    public string GetFormattedTime()
    {
        int hours = (int)(playTime / 3600);
        int minutes = (int)((playTime % 3600) / 60);
        int seconds = (int)(playTime % 60);
        return $"{hours}:{minutes:D2}:{seconds:D2}";
    }

    public float GetPlayTime()
    {
        return playTime;
    }
}
