using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class AttackObj : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator anim;
    [SerializeField] RuntimeAnimatorController defalutController;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool isPlayer;
    [SerializeField] CircleCollider2D circleCollider;
    EffectData effectData;
    AttackPoolManager poolManager;
    EffectPoolManager effectPoolManager;
    float damage;
    Vector2 dir;
    float shootSpeed;
    float coolTime;

    Coroutine coroutine;

    public bool IsActive {  get; private set; }

    public void InitData(Sprite sprite, float value, Vector2 vec, float speed, float cool, AttackPoolManager manager, bool isPlayerAttack, Vector2 colliderOffset, float colliderRadius, AttackData attackData = null)
    {
        Debug.Log($"[AttackObj.InitData] vec: {vec}, speed: {speed}");

        IsActive = true;

        if (attackData == null)
            spriteRenderer.sprite = sprite;
        else
        {
            spriteRenderer.sprite = attackData.sprite;
            anim.runtimeAnimatorController = attackData.animationController;
            effectData = attackData.effect;
        }
        damage = value;
        dir = vec;
        shootSpeed = speed;
        coolTime = cool;
        poolManager = manager;
        effectPoolManager = poolManager.effectPoolManager;
        circleCollider.offset = colliderOffset;
        circleCollider.radius = colliderRadius;

        isPlayer = isPlayerAttack;
        transform.right = dir;
        
        StartDurate();
    }

    void StartDurate()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(DurateAttack());
    }

    IEnumerator DurateAttack()
    {
        yield return new WaitForSeconds(coolTime);
        poolManager.ReturnPool(this);
    }


    private void FixedUpdate()
    {
        Debug.Log($"FixedUpdate(): dir = {dir}, shootSpeed = {shootSpeed}, Result = {dir * shootSpeed}");
        rb.velocity = dir * shootSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 vec = collision.ClosestPoint(transform.position);
        bool isWall = ((1 << collision.gameObject.layer) & WallLayer) != 0;
        if(isWall || collision.CompareTag("Collision"))
        {
            if (isPlayer)
            {
                PlayerEffect(vec);
            }

            poolManager.ReturnPool(this);
            return;
        }

        Character character;
        if (isPlayer)
            character = collision.GetComponent<Monster>();
        else
            character = collision.GetComponent<Player>();

        if (character == null) return;

        if(isPlayer)
        {
            PlayerEffect(vec);
        }

        character.TakeDamage(damage);

        poolManager.ReturnPool(this);
        
    }

    public void ResetObj()
    {
        rb.velocity = Vector2.zero;
        anim.runtimeAnimatorController = defalutController;

        IsActive = false;
    }

    void PlayerEffect(Vector3 vec)
    {
        ShowEffect(vec);
        AudioManager.Instance.PlayEffect(HitSfx);
    }

    public void ShowEffect(Vector3 vec)
    {
        Effect effect = effectPoolManager.GetObj();
        effect.transform.position = vec;
        effect.InitData(effectPoolManager, effectData, dir);
    }
}
