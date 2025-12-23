using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstValue;

public class StartSceneCanvasManager : MonoBehaviour
{
    [SerializeField] GameObject soundPopup;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    AudioManager audioManager;
    SaveLoadManager saveLoadManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        saveLoadManager = SaveLoadManager.Instance;
        InitSoundSlider();
    }

    public void GameStartButton()
    {
        LoadingManager.LoadScene(GameScene);
    }

    public void InitSoundSlider()
    {
        bgmSlider.value = saveLoadManager.datas.soundData.bgmVolume;
        sfxSlider.value = saveLoadManager.datas.soundData.sfxVolume;
    }

    public void ChangeBgmVolume()
    {
        audioManager.UpdateBgmVolume(bgmSlider.value);
    }

    public void ChangeSfxVolume()
    {
        audioManager.UpdateEffectVolume(sfxSlider.value);
    }

    public void CloseSoundPopup()
    {
        saveLoadManager.SaveDatas();
        soundPopup.SetActive(false);
    }
}
