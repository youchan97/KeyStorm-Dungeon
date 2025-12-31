using System.Collections;
using UnityEditor.EditorTools;
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
    public bool PlaySfx(AudioClip clip, float volume, bool isButton = false)
    {
        if (clip == null) return false;

        if(isButton == false)
        {
            if (!poolManager.TryRegister(clip)) return false;
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
        poolManager.Unregister(currentClip);
        currentClip = null;
        sfxAudio.ignoreListenerPause = false;
        poolManager.ReturnObject(this);
    }
}
