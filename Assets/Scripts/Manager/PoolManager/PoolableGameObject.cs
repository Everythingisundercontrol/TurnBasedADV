using System;
using UnityEngine;

public class PoolableGameObject : MonoBehaviour, IPoolableObject
{
    public float LastUsedTime { get; private set; }
    public bool Active { get; private set; }


    protected virtual void OnDestroy()
    {
        // 销毁时自动归还对象，避免内存泄漏
        PoolManager.Instance.ReturnObject(this);
    }

    public virtual void OnActivate()
    {
        Active = true;
        LastUsedTime = Time.time;
    }

    public virtual void OnDeactivate()
    {
        Active = false;
        LastUsedTime = Time.time;
    }

    public void OnIdleDestroy()
    {
        Destroy(gameObject);
    }
}