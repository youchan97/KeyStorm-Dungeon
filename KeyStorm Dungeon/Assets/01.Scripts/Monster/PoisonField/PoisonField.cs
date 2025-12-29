using UnityEngine;

public class PoisonField : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float poisonFieldDuration;

    private float currentAttackCooldown;
    private float currentFieldDuration;

    private void Start()
    {
        currentAttackCooldown = 0.5f;
        currentFieldDuration = poisonFieldDuration;
    }

    private void Update()
    {
        currentAttackCooldown -= Time.deltaTime;
        currentFieldDuration -= Time.deltaTime;

        if(currentFieldDuration <= 0)
        {
            Destroy(gameObject);
        }
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
