using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public class StartSceneCanvasManager : MonoBehaviour
{
    public void OpenSoundSetting()
    {
        UiManager.Instance.OpenSoundPopup();
    }

    public void GameStartButton()
    {
        StageDataManager.Instance.SelectDifficulty(StageDifficulty.Easy);
        AudioManager.Instance.PlayButton();
        LoadingManager.LoadScene(GameScene);
    }
}
