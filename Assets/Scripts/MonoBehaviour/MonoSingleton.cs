using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A MonoSingleton needs to be abstract, of generic T type, inherits Monobehaviour where T type inherits MonoSingleton of type T
// Any other class can now inherit MonoSingleton of type of the class that inherits it
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.Log($"{typeof(T).ToString()} is null");
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;

        Init();
    }

    public virtual void Init()
    {
        // Optional to override
    }
}
