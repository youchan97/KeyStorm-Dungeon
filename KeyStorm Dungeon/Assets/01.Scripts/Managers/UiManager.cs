using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : SingletonManager<UiManager>
{
    [SerializeField] GameObject soundPanel;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    AudioManager audioManager;
    SaveLoadManager saveLoadManager;

    Stack<GameObject> popupStack = new Stack<GameObject>();

    private void Start()
    {
        audioManager = AudioManager.Instance;
        saveLoadManager = SaveLoadManager.Instance;
        InitSoundSlider();
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

    public void OpenSoundPopup()
    {
        OpenPopup(soundPanel);
    }

    public void CloseSoundPopup()
    {
        saveLoadManager.SaveDatas();
        ClosePopup();
    }

    public void OpenPopup(GameObject obj)
    {
        popupStack.Push(obj);
        obj.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupStack.Count == 0) return;

        GameObject obj = popupStack.Pop();
        obj.SetActive(false);
    }

    public void CloseAllPopup()
    {
        if (popupStack.Count == 0) return;

        while (popupStack.Count > 0)
            ClosePopup();
    }

    public void ClearStack() => popupStack.Clear();
}
