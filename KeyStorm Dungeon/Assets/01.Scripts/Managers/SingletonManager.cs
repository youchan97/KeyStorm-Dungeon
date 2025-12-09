using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }

    public static void SetupInstance()
    {
        instance = FindAnyObjectByType<T>();
        if (instance == null)
        {
            instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
