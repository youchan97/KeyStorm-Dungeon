using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class Dynamite : MonoBehaviour
{
    int damage;
    float radius;
    float duration;
    EffectData effectData;
    [SerializeField] LayerMask layerMask;

    EffectPoolManager effectPoolManager;

    Coroutine explodeCoroutine;

    private void OnDisable()
    {
        explodeCoroutine = null;
    }

    public void InitData(DynamiteData data, Player player)
    {
        damage = data.damage;
        radius = data.radius;
        duration = data.duration;
        effectData = data.effect;

        effectPoolManager = player.EffectPoolManager;

        if (explodeCoroutine != null)
        {
            StopCoroutine(explodeCoroutine);
            explodeCoroutine = null;
        }

        explodeCoroutine = StartCoroutine(WaitExplode());
    }

    IEnumerator WaitExplode()
    {
        yield return new WaitForSeconds(duration);
        Explode();
        explodeCoroutine = null;
    }

    void Explode()
    {
        Effect effect = effectPoolManager.GetObj();
        effect.transform.position = transform.position;
        effect.InitData(effectPoolManager, effectData, Vector2.zero, radius);
        AudioManager.Instance.PlayEffect(BombSfx);
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D col in cols)
        {
            Character character = col.GetComponent<Character>();

            if (character == null)
                continue;

            character.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
