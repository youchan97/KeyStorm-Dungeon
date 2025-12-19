using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : SingletonManager<ItemDropManager>
{
    [Header("아이템 프리팹")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject bombPrefab;

    [Header("드랍 세팅")]
    [SerializeField] private float dropForce;
    [SerializeField] private float dropSpread;

    private const string GOLD_POOL_NAME = "GoldItem";
    private const string BOMB_POOL_NAME = "BombItem";

    [System.Serializable]
    public class BombDropSetting
    {
        public MonsterTier monsterTier;
        [Range(0f, 1f)] public float dropChance;
    }

    [Header("티어별 폭탄 드랍")]
    [SerializeField] private List<BombDropSetting> bombDropSettings = new List<BombDropSetting>();

    protected override void Awake()
    {
        base.Awake();
        if (goldPrefab == null)
        {
            Debug.LogError("ItemDropManager: 골드 프리팹이 할당되지 않음.");
        }

        if (bombPrefab == null)
        {
            Debug.LogError("ItemDropManager: 폭탄 프리팹이 할당되지 않음.");
        }
    }

    public void DropItems(Vector3 dropPosition, MonsterData monsterData)
    {
        int goldToDrop = Random.Range(monsterData.minDropGold, monsterData.maxDropGold + 1);

        for (int i = 0; i < goldToDrop; i++)
        {
            DropGold(dropPosition);
        }
        
        ApplyTierBasedBombDrop(dropPosition, monsterData.tier);
    }

    private void DropGold(Vector3 position)
    {
        GoldPickup goldPickup = ObjectPoolManager.Instance.GetObject<GoldPickup>(GOLD_POOL_NAME, position, Quaternion.identity);
        if (goldPickup == null) return;

        Rigidbody2D rb = goldPickup.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 scatterOffset = Random.insideUnitCircle * dropSpread;
            rb.AddForce((randomDirection + scatterOffset).normalized * dropForce, ForceMode2D.Impulse);
        }
    }

    private void ApplyTierBasedBombDrop(Vector3 position, MonsterTier monsterTier)
    {
        BombDropSetting setting = bombDropSettings.Find(s => s.monsterTier == monsterTier);

        if (setting == null)
        {
            return;
        }

        if(Random.value <= setting.dropChance)
        {
            DropBomb(position);
        }
    }

    private void DropBomb(Vector3 position)
    {
        BombPickup bombPickup = ObjectPoolManager.Instance.GetObject<BombPickup>(BOMB_POOL_NAME, position, Quaternion.identity);
        if (bombPrefab == null) return;

        Rigidbody2D rb = bombPickup.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDirection = (Random.insideUnitCircle + Random.insideUnitCircle * dropSpread).normalized;
            rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
        }
    }

    public void ReturnGoldPickup(GoldPickup pickup)
    {
        ObjectPoolManager.Instance.ReturnObject(pickup.gameObject, GOLD_POOL_NAME);
    }

    public void ReturnBombPickup(BombPickup pickup)
    {
        ObjectPoolManager.Instance.ReturnObject(pickup.gameObject, BOMB_POOL_NAME);
    }
}
