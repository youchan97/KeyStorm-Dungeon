using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private Vector2 targetDirection;
    private GameObject projectilePrefab;
    private AttackPoolManager objectPoolManager;

    public void Initialize(Transform target, float speed, int damage, GameObject projectilePrefab)
    {
        if (target != null)
        {
            targetDirection = (target.position - transform.position).normalized;
        }
        else
        {
            targetDirection = Vector2.left;
            Debug.LogWarning("MonsterProjectile: target이 없음. 기본 설정된 방향으로 발사");
        }

        this.speed = speed;
        this.damage = damage;
    }

    private void FixedUpdate()
    {
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject currentGameObject = collision.gameObject;

        if (collision.CompareTag("Player"))
        {
            Player player = currentGameObject.GetComponent<Player>();

            if(player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("원거리 공격에 맞음");
                Destroy(this);
                //objectPoolManager.ReturnPool();
            }
        }

        if (collision.CompareTag("collision"))
        {
            Debug.Log("원거리 공격이 벽에 충돌함");
            Destroy(this);
            //objectPoolManager.ReturnPool();
        }
    }
}
