using UnityEngine;
using UnityEngine.Pool;

public class SfxPoolManager : SingletonManager<SfxPoolManager>
{
    [SerializeField] private SfxPool sfxPrefab;
    [SerializeField] private int poolSize = 10;

    private IObjectPool<SfxPool> pool;
    public IObjectPool<SfxPool> SfxPool
    {
        get
        {
            if (pool == null) 
            {
                pool = new ObjectPool<SfxPool>(
                    CreateSfx, OnTakeFromPool, OnReturnToPool, null, 
                    defaultCapacity: poolSize
                    );
            }
            return pool;
        }
    }

    public SfxPool GetObject()
    {
        return SfxPool.Get();
    }
    public void ReturnObject(SfxPool sfx)
    {
        SfxPool.Release(sfx);
    }

    private SfxPool CreateSfx()
    {
        var sfx = Instantiate(sfxPrefab, transform);
        sfx.gameObject.SetActive(false);
        return sfx;
    }

    private void OnTakeFromPool(SfxPool sfx)
    {
        sfx.gameObject.SetActive(true);
    }

    private void OnReturnToPool(SfxPool sfx)
    {
        sfx.gameObject.SetActive(false);
    }
}
