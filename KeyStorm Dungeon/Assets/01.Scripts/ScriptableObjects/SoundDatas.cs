using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/Sound")]
public class SoundDatas : ScriptableObject
{
    public List<AudioData> audioClips;
}

[System.Serializable]
public class AudioData
{
    public string audioKey;
    public AudioClip audioClip;
}
[System.Serializable]
public class SoundData
{
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;
}
