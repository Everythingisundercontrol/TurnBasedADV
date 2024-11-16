using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : BaseSingleton<PoolManager>, IMonoManager
{
    private Dictionary<Type, IObjectPool> _pools;


    public void OnInit()
    {
        _pools = new Dictionary<Type, IObjectPool>();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
        //定期检查并自动销毁闲置时间过长的对象
        foreach (var pool in _pools.Values)
        {
            pool.AutoDestroy();
        }
    }

    public void OnClear()
    {
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="initialSize">初始化对象数量</param>
    /// <param name="objectGenerator">生成对象的方法</param>
    /// <param name="maxIdleTime">对象最大闲置时间</param>
    /// <param name="checkInterval">对象池自动销毁时间间隔，每次销毁10%闲置对象</param>
    /// <typeparam name="T">对象类型</typeparam>
    public void CreatePool<T>(int initialSize, Func<T> objectGenerator, float maxIdleTime = 30f,
        float checkInterval = 10f) where T : IPoolableObject
    {
        var type = typeof(T);
        if (_pools.ContainsKey(type))
        {
            Debug.LogWarning("已有对象池" + typeof(T));
            return;
        }

        var pool = new ObjectPool<T>(initialSize, objectGenerator, maxIdleTime, checkInterval);
        _pools.Add(type, pool);
    }

    /// <summary>
    /// 获取对象池
    /// </summary>
    public ObjectPool<T> GetPool<T>() where T : IPoolableObject
    {
        var type = typeof(T);
        if (_pools.TryGetValue(type, out var pool))
        {
            return pool as ObjectPool<T>;
        }

        return null;
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    public T GetObject<T>() where T : IPoolableObject
    {
        var pool = GetPool<T>();
        if (pool != null)
        {
            return pool.Get();
        }

        Debug.LogError("没有这个对象的对象池");
        return default;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    public void ReturnObject<T>(T obj) where T : IPoolableObject
    {
        var pool = GetPool<T>();
        pool?.ReturnToPool(obj);
    }
}