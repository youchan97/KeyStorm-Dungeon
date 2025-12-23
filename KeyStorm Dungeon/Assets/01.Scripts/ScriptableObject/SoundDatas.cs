using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObject/Sound")]
public class SoundDatas : ScriptableObject
{
    public List<AudioData> audioClips;
}

[System.Serializable]
public class AudioData
{
    public string audioKey;
    public List<AudioClip> audioClip;
}
[System.Serializable]
public class SoundData
{
    public float bgmVolume = DefaultBgmVolume;
    public float sfxVolume = DefaultSfxVolume;
}
