using System.Collections;
using UnityEngine;

public class SfxPool : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudio;
    SfxPoolManager poolManager;

    AudioClip currentClip;

    private void Awake()
    {
        poolManager = SfxPoolManager.Instance;
    }

    public bool PlaySfx(AudioClip clip, float volume, bool isButton = false, bool isLoop = false)
    {
        if (clip == null) return false;

        if(isButton == false)
        {
            if (!poolManager.TryRegister(clip, isLoop))
                return false;
        }

        sfxAudio.ignoreListenerPause = isButton;

        currentClip = clip;

        sfxAudio.clip = clip;
        sfxAudio.volume = volume;
        sfxAudio.Play();

        StartCoroutine(WaitReturnPool(isButton));

        return true;
    }
    IEnumerator WaitReturnPool(bool isButton = false)
    {
        if (isButton)
            yield return new WaitForSecondsRealtime(sfxAudio.clip.length);
        else
            yield return new WaitForSeconds(sfxAudio.clip.length);
        ResetSfx();
    }

    void ResetSfx()
    {
        poolManager.Unregister(currentClip);
        currentClip = null;
        sfxAudio.loop = false;
        sfxAudio.ignoreListenerPause = false;
        poolManager.ReturnObject(this);
    }


    public void PlayLoopSfx(AudioClip clip, float volume)
    {
        if (clip == null) return;

        sfxAudio.clip = clip;
        sfxAudio.volume = volume;
        sfxAudio.loop = true;

        currentClip = clip;
        sfxAudio.Play();
    }

    public void StopLoopSfx()
    {
        if (currentClip == null) return;

        sfxAudio.Stop();
        sfxAudio.loop = false;

        poolManager.Unregister(currentClip);
        currentClip = null;

        poolManager.ReturnObject(this);
    }
}
