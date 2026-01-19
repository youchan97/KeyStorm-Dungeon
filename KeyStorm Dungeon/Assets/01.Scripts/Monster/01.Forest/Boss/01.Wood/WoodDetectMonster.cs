using UnityEngine;

public class WoodDetectMonster : MonoBehaviour
{
    [SerializeField] private Wood wood;

    private void Awake()
    {
        wood = GetComponentInParent<Wood>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (wood.IsDash)
        {
            if (((1 << collision.gameObject.layer) & wood.HitToDashStopLayer.value) > 0)
            {
                if (((1 << collision.gameObject.layer) & wood.RootLayer.value) > 0)
                {
                    WoodsRoot hitRoot = collision.GetComponent<WoodsRoot>();
                    if (hitRoot != null)
                    {
                        hitRoot.Die();
                        wood.StopDash();
                    }
                }
                else
                {
                    wood.StopDash();
                }
            }
        }
    }
}
