using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }

        if (_instance == null)
        {
            _instance = this as T;
        }
    }
}