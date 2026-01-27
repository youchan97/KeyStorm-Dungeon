using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnRockBreak()
    {
        Destroy(gameObject);
    }
}
