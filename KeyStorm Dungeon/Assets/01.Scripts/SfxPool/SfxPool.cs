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
    public void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (!poolManager.TryRegister(clip))
            return;

        currentClip = clip;

        sfxAudio.clip = clip;
        sfxAudio.volume = volume;
        sfxAudio.Play();

        StartCoroutine(WaitReturnPool());
    }
    IEnumerator WaitReturnPool()
    {
        yield return new WaitForSeconds(sfxAudio.clip.length);
        poolManager.Unregister(currentClip);
        currentClip = null;
        poolManager.ReturnObject(this);
    }
}
