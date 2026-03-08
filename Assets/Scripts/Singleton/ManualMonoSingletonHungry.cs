using System;
using UnityEngine;


/// <summary>
/// 饿汉Mono单例模板
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManualMonoSingletonHungry<T> : MonoBehaviour  where T : ManualMonoSingletonHungry<T>
{
    private static T _instance;

    public static T Instance => _instance;

    public virtual void Awake()
    {
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
            
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }
}