using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class AudioManager : SingletonManager<AudioManager>
{
    [SerializeField] AudioSource bgmAudio;

    [SerializeField] SoundDatas bgmData;
    [SerializeField] SoundDatas sfxData;
    [SerializeField] float sfxVolume;

    Dictionary<string, List<AudioClip>> bgmClips = new Dictionary<string, List<AudioClip>>();
    Dictionary<string, List<AudioClip>> sfxClips = new Dictionary<string, List<AudioClip>>();

    SfxPoolManager sfxPoolManager;
    SaveLoadManager saveLoadManager;

    protected override void Awake()
    {
        base.Awake();
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
        bgmAudio.volume = saveLoadManager.datas.soundData.bgmVolume;
        sfxVolume = saveLoadManager.datas.soundData.sfxVolume;
    }

    public void PlayBgm(string audioName)
    {
        if (!bgmClips.ContainsKey(audioName)) return;

        if (bgmClips[audioName].Count == 0) return;

        int clipIndex = bgmClips[audioName].Count > 1 ? Random.Range(0, bgmClips[audioName].Count) : 0;

        AudioClip bgmClip = bgmClips[audioName][clipIndex];
        bgmAudio.clip = bgmClip;
        bgmAudio.Play();
    }


    public void PlayEffect(string audioName, bool isButton = false)
    {
        if (!sfxClips.ContainsKey(audioName)) return;

        if (sfxClips[audioName].Count == 0) return;

        int clipIndex = sfxClips[audioName].Count > 1 ? Random.Range(0, sfxClips[audioName].Count) : 0;

        AudioClip sfxClip = sfxClips[audioName][clipIndex];
        var sfx = sfxPoolManager.GetObject();
        if(!sfx.PlaySfx(sfxClip, sfxVolume, isButton))
        {
            sfxPoolManager.ReturnObject(sfx);
        }
    }

    public void PlayButton() => PlayEffect(ButtonSfx, true);

    public void UpdateBgmVolume(float value)
    {
        bgmAudio.volume = value;

        if (saveLoadManager == null) return;

        saveLoadManager.datas.soundData.bgmVolume = value;
    }

    public void UpdateEffectVolume(float value)
    {
        sfxVolume = value;

        if (saveLoadManager == null) return;

        saveLoadManager.datas.soundData.sfxVolume = value;
    }
}
