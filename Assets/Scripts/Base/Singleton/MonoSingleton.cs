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
        if (_instance)
        {
            Debug.Log("destroy : " + gameObject.name);
            Destroy(gameObject);
        }

        if (!_instance)
        {
            _instance = this as T;
        }
    }
}