using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : BaseSingleton<EventManager>, IMonoManager
{
    private readonly Dictionary<EventName, List<(Delegate listener, int priority)>> _eventDic =
        new Dictionary<EventName, List<(Delegate, int)>>();

    public delegate void YuEvent();

    public delegate void YuEvent<in T1>(T1 t1); //泛型类型参数默认是不变的，用in来声明类型参数的逆变性，委托类型是逆变的

    public delegate void YuEvent<in T1, in T2>(T1 t1, T2 t2);

    public delegate void YuEvent<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);

    public void OnInit()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }

    /// <summary>
    /// 添加一个事件的监听者
    /// </summary>
    public void AddListener(EventName eventName, YuEvent action, int priority = 0)
    {
        OnListenerAdding(eventName, action, priority);
    }

    public void AddListener<T1>(EventName eventName, YuEvent<T1> action, int priority = 0)
    {
        OnListenerAdding(eventName, action, priority);
    }

    public void AddListener<T1, T2>(EventName eventName, YuEvent<T1, T2> action, int priority = 0)
    {
        OnListenerAdding(eventName, action, priority);
    }

    public void AddListener<T1, T2, T3>(EventName eventName, YuEvent<T1, T2, T3> action, int priority = 0)
    {
        OnListenerAdding(eventName, action, priority);
    }


    /// <summary>
    /// 移除一个事件的监听者
    /// </summary>
    public void RemoveListener(EventName eventName, YuEvent action, int priority = 0)
    {
        OnListenerRemoving(eventName, action, priority);
    }

    public void RemoveListener<T1>(EventName eventName, YuEvent<T1> action, int priority = 0)
    {
        OnListenerRemoving(eventName, action, priority);
    }

    public void RemoveListener<T1, T2>(EventName eventName, YuEvent<T1, T2> action, int priority = 0)
    {
        OnListenerRemoving(eventName, action, priority);
    }

    public void RemoveListener<T1, T2, T3>(EventName eventName, YuEvent<T1, T2, T3> action, int priority = 0)
    {
        OnListenerRemoving(eventName, action, priority);
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void Dispatch(EventName eventName)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            return;
        }

        foreach (var (action, _) in _eventDic[eventName])
        {
            if (action is YuEvent yuEvent)
            {
                yuEvent.Invoke();
                continue;
            }

            Debug.LogError($"事件参数类型错误 eventId: {eventName} " +
                           $"/ 期待的action类型是 yuEvent " +
                           $"/ 实际的action类型是 {action.GetType().Name}");
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Dispatch<T1>(EventName eventName, T1 param1)
    {
        if (!_eventDic.ContainsKey(eventName)) return;

        foreach (var (action, _) in _eventDic[eventName])
        {
            if (action is YuEvent<T1> yuEvent)
            {
                yuEvent.Invoke(param1);
                continue;
            }

            Debug.LogError($"事件参数类型错误 eventId: {eventName} " +
                           $"/ 期待的action类型是 yuEvent<{typeof(T1).Name}> " +
                           $"/ 实际的action类型是 {action.GetType().Name}");
        }
    }

    public void Dispatch<T1, T2>(EventName eventName, T1 param1, T2 param2)
    {
        if (!_eventDic.ContainsKey(eventName)) return;

        foreach (var (action, _) in _eventDic[eventName])
        {
            if (action is YuEvent<T1, T2> yuEvent)
            {
                yuEvent.Invoke(param1, param2);
                continue;
            }

            Debug.LogError($"事件参数类型错误 eventId: {eventName} " +
                           $"/ 期待的action类型是 yuEvent<{typeof(T1).Name}{typeof(T2).Name}> " +
                           $"/ 实际的action类型是 {action.GetType().Name}");
        }
    }

    public void Dispatch<T1, T2, T3>(EventName eventName, T1 param1, T2 param2, T3 param3)
    {
        if (!_eventDic.ContainsKey(eventName)) return;

        foreach (var (action, _) in _eventDic[eventName])
        {
            if (action is YuEvent<T1, T2, T3> yuEvent)
            {
                yuEvent.Invoke(param1, param2, param3);
                continue;
            }

            Debug.LogError($"事件参数类型错误 eventId: {eventName} " +
                           $"/ 期待的action类型是 yuEvent<{typeof(T1).Name}、{typeof(T2).Name}、{typeof(T3).Name}> " +
                           $"/ 实际的action类型是 {action.GetType().Name}");
        }
    }

    /// <summary>
    /// 查询当前eventName的所有已注册action及其优先度
    /// </summary>
    public void PrintEventPriority(EventName eventName)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            Debug.Log($"没有注册任何action {eventName}");
            return;
        }

        Debug.Log($"已注册的事件：{eventName}->");
        foreach (var (action, priority) in _eventDic[eventName])
        {
            Debug.Log($"Priority: {priority}, Action: {action.Method.Name}");
        }
    }

    /// <summary>
    /// 注册事件监听时
    /// </summary>
    private void OnListenerAdding(EventName eventName, Delegate listener, int priority)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            _eventDic[eventName] = new List<(Delegate, int)>();
        }

        var index = FindInsertIndex(eventName, priority);
        _eventDic[eventName].Insert(index, (listener, priority));
    }

    /// <summary>
    /// 依据优先度获取插入位置，二分查找
    /// </summary>
    private int FindInsertIndex(EventName eventName, int priority)
    {
        var list = _eventDic[eventName];
        var low = 0;
        var high = list.Count - 1;
        while (low <= high)
        {
            var mid = (low + high) / 2;
            if (list[mid].priority < priority)
            {
                high = mid - 1;
                continue;
            }

            low = mid + 1;
        }

        return low;
    }

    /// <summary>
    /// 移除监听的事件时
    /// </summary>
    private void OnListenerRemoving(EventName eventName, Delegate listener, int priority)
    {
        if (!_eventDic.ContainsKey(eventName))
        {
            return;
        }

        for (var i = 0; i < _eventDic[eventName].Count; i++)
        {
            if (_eventDic[eventName][i].priority != priority || _eventDic[eventName][i].listener != listener)
            {
                continue;
            }

            _eventDic[eventName].RemoveAt(i);
            break;
        }

        if (_eventDic[eventName].Count == 0)
        {
            _eventDic.Remove(eventName);
        }
    }
}