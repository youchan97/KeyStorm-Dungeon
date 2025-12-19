using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonManager<AudioManager>
{
    [SerializeField] AudioSource bgmAudio;

    [SerializeField] SoundDatas bgmData;
    [SerializeField] SoundDatas sfxData;

    Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    SfxPoolManager sfxPoolManager;
    SaveLoadManager saveLoadManager;

    protected override void Awake()
    {
        base.Awake();
        saveLoadManager = SaveLoadManager.Instance;
        initAudioDic();

    }
    private void Start()
    {
        saveLoadManager = SaveLoadManager.Instance;
        sfxPoolManager = SfxPoolManager.Instance;
        LoadVolume();
    }

    void initAudioDic() //사운드 초기화
    {
        for(int i = 0; i<bgmData.audioClips.Count; i++)
        {
            AudioData audioData = bgmData.audioClips[i];
            bgmClips.Add(audioData.audioKey, audioData.audioClip);
        }
        for (int i = 0; i < sfxData.audioClips.Count; i++)
        {
            AudioData audioData = sfxData.audioClips[i];
            sfxClips.Add(audioData.audioKey, audioData.audioClip);
        }
    }
    void LoadVolume() //볼륨값
    {
        //bgmAudio.volume = saveLoadManager.datas.soundData.bgmVolume;
    }

    public void PlayBgm(string audioName)
    {
        if (!bgmClips.ContainsKey(audioName)) return;
        bgmAudio.clip = bgmClips[audioName];
        bgmAudio.Play();
    }

    public void PlayEffect(string audioName, float volume = 0.5f)
    {
        if (!sfxClips.ContainsKey(audioName)) return;
        AudioClip audioClip = sfxClips[audioName];
        var sfx = sfxPoolManager.GetObject();
        sfx.PlaySfx(audioClip, volume);
    }

    public void UpdateBgmVolume(float value)
    {
        bgmAudio.volume = value;
        saveLoadManager.datas.soundData.bgmVolume = value;
    }

    public void UpdateEffectVolume(float value)
    {
        saveLoadManager.datas.soundData.sfxVolume = value;
    }
}
