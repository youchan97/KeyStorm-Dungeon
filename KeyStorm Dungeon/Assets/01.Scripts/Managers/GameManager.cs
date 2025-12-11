using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private bool isStart;
    private bool isPaused;

    public void GameStart()
    {
        isStart = true;
        isPaused = false;
        Time.timeScale = 1f;

    }
    public void Pause()
    {
        isStart = false;
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    public void GameOver()
    {
        isStart = false;
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
