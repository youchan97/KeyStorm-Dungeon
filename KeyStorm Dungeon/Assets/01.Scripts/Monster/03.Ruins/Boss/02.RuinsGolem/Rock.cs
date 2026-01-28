using UnityEngine;
using static ConstValue;

public class Rock : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    private void OnRockBreak()
    {
        audioManager.PlayEffect(RockDropSfx);
        Destroy(gameObject);
    }
}
