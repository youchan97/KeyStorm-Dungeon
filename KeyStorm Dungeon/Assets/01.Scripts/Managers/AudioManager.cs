using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonManager<AudioManager>
{
    [SerializeField] AudioSource bgmAudio;
    
    Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();

    }
    private void Start()
    {
        
    }

    void initAudioDic()
    {
    }
    void LoadVolume()
    {

    }

    public void PlayBgm(string audioName)
    {
        if (!bgmClips.ContainsKey(audioName)) return;
        bgmAudio.clip = bgmClips[audioName];
        bgmAudio.Play();
    }

    public void PlayEffect(string audioName)
    {
        if (!sfxClips.ContainsKey(audioName)) return;
        AudioClip audioClip = sfxClips[audioName];
        
    }

    public void UpdateBgmVolume(float value)
    {
        bgmAudio.volume = value;
    }

    public void UpdateEffectVolume(float value)
    {

    }
}
