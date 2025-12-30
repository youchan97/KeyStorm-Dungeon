using System.Collections;
using UnityEngine;

public class PoisonField : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float poisonFieldDuration;

    [SerializeField] private string poolName = "PoisonField";

    private float currentAttackCooldown;
    private float currentFieldDuration;


    private void Update()
    {
        currentAttackCooldown -= Time.deltaTime;
        currentFieldDuration -= Time.deltaTime;
    }

    public void Initialize(float duration)
    {
        currentFieldDuration = duration;
        currentAttackCooldown = 0.5f;

        StartCoroutine(DurationCoroutine());
    }

    private IEnumerator DurationCoroutine()
    {
        yield return new WaitForSeconds(currentFieldDuration);

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnObject(gameObject, poolName);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if ( currentAttackCooldown <= 0f)
        {
            if (currentGameObject.CompareTag("Player"))
            {
                Player player = currentGameObject.GetComponent<Player>();

                if ( player != null)
                {
                    player.TakeDamage(1);
                    currentAttackCooldown = attackCooldown;
                }
            }
        }
    }
}
