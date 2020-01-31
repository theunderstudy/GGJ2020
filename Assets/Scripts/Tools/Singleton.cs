using UnityEngine;
using System;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError(String.Format("{0} instance already exists", typeof(T).Name));
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}
