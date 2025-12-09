using System.Collections;
using UnityEngine;

public class SfxPool : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudio;
    SfxPoolManager PoolManager;

    private void Awake()
    {
        PoolManager = SfxPoolManager.Instance;
    }
    public void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null) return;
        sfxAudio.clip = clip;
        sfxAudio.volume = volume;
        sfxAudio.Play();

        StartCoroutine(WaitReturnPool());
    }
    IEnumerator WaitReturnPool()
    {
        yield return new WaitForSeconds(sfxAudio.clip.length);
        PoolManager.ReturnObject(this);
    }
}
