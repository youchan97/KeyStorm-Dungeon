using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    float damage;
    float radius;
    float duration;
    LayerMask enemyLayer;
    EffectPoolManager effectPoolManager;
    EffectData effectData;

    float shakePower;
    float shakeDuration;

    [SerializeField] GameObject meteoBall;

    Tween tween;
    private void Start()
    {
        tween?.Kill();

        tween = meteoBall.transform.DOMove(transform.position, duration).OnComplete(Explode);
    }

    private void OnDisable()
    {
        tween?.Kill();
    }


    public void InitData(Player player, MeteoData data)
    {
        damage = player.PlayerAttack.Damage * player.PlayerAttack.DamageMultiple * data.damageMultiple;
        radius = data.radius;
        duration = data.duration;
        enemyLayer = data.enemyLayer;
        effectPoolManager = player.EffectPoolManager;
        effectData = data.explodeEffect;

        shakePower = data.shakePower;
        shakeDuration = data.shakeDuration;
    }

    void Explode()
    {
        Effect effect = effectPoolManager.GetObj();
        effect.transform.position = transform.position;
        effect.InitData(effectPoolManager, effectData, Vector2.zero, radius);

        ShakeCameraEvent.StartShakeCamera(shakePower, shakeDuration);

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        for (int i = 0; i < cols.Length; i++)
        {
            Monster monster = cols[i].GetComponent<Monster>();

            if (monster != null)
            {
                monster.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}


