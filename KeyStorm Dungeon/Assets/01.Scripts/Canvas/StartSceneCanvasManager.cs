using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public class StartSceneCanvasManager : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    public void OpenSoundSetting()
    {
        UiManager.Instance.OpenSoundPopup();
    }

    public void GameStartButton()
    {
        LoadingManager.LoadScene(GameScene);
    }
}
