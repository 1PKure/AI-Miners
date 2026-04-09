using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool isPersistent = true;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<T>();

            if (instance == null)
                instance = new GameObject(typeof(T).Name).AddComponent<T>();

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (isPersistent)
                DontDestroyOnLoad(gameObject);

            return;
        }

        if (instance != this)
            Destroy(gameObject);
    }
}