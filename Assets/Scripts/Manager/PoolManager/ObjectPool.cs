using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool where T : IPoolableObject
{
    private readonly Stack<T> _poolStack; //存储闲置对象的堆栈
    private readonly List<T> _activeObjects; //存储活跃对象的列表
    private readonly int _initialSize; //对象池的初始化对象的数量，也是对象池最小保留对象的数量
    private readonly float _maxIdleTime; //对象的最大闲置时间
    private readonly float _checkInterval; //检查闲置对象的时间间隔
    private float _lastCheckTime; //上一次检查的时间
    private readonly Func<T> _objectGenerator;


    public ObjectPool(int initialSize, Func<T> objectGenerator, float maxIdleTime = 30f, float checkInterval = 10f)
    {
        _objectGenerator = objectGenerator;
        _initialSize = initialSize;
        _maxIdleTime = maxIdleTime;
        _checkInterval = checkInterval;
        _poolStack = new Stack<T>(_initialSize);
        _activeObjects = new List<T>(_initialSize);
        for (var i = 0; i < _initialSize; i++)
        {
            _poolStack.Push(CreateNewObject());
        }

        // Debug.Log(typeof(T)+"     "+_poolStack.Count);
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <returns></returns>
    public T Get()
    {
        if (_poolStack.Count == 0)
        {
            _poolStack.Push(CreateNewObject());
        }

        var obj = _poolStack.Pop();
        if (obj.Active)
        {
            Debug.LogWarning("错误引用，弹出了活跃的obj，尝试修正stack");
            return Get();
        }

        obj.OnActivate();
        _activeObjects.Add(obj);
        return obj;
    }

    /// <summary>
    /// 归还对象到池中
    /// </summary>
    public void ReturnToPool(T obj)
    {
        if (!obj.Active)
        {
            Debug.LogWarning("错误引用，或回收了闲置的obj，可能重复回收");
            return;
        }

        obj.OnDeactivate();
        _activeObjects.Remove(obj);
        _poolStack.Push(obj);
    }

    /// <summary>
    /// 定期检查并自动销毁闲置时间过长的对象
    /// </summary>
    public void AutoDestroy()
    {
        var currentTime = Time.time;
        if (!(currentTime - _lastCheckTime > _checkInterval))
        {
            return;
        }

        _lastCheckTime = currentTime;

        //处理活跃对象
        foreach (var obj in _activeObjects)
        {
        }

        //处理闲置对象
        if (_poolStack.Count <= _initialSize) //保留初始化的obj数量
        {
            return;
        }

        var objectsToDestroy = Math.Max(1, _poolStack.Count / 10); //每次至少销毁一个闲置对象，每次最多销毁闲置对象的10%
        for (var i = 0; i < objectsToDestroy; i++)
        {
            if (_poolStack.Count <= 0)
            {
                continue;
            }

            var obj = _poolStack.Pop();
            if (currentTime - obj.LastUsedTime > _maxIdleTime)
            {
                obj.OnIdleDestroy();
                //不压回栈中以解除引用，gc识别释放内存
                Debug.Log("销毁obj");
                continue;
            }

            //对象仍在使用，将其放回堆栈
            _poolStack.Push(obj);
            break;
        }
    }

    /// <summary>
    /// 创建新的对象实例
    /// </summary>
    /// <returns></returns>
    private T CreateNewObject()
    {
        return _objectGenerator();
    }
}