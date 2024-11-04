// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// public class InputEvent : BaseSingleton<InputEvent>
// {
//     // private List<Action<Vector3>> _actionOnClickLeft;
//     //
//     //
//     // /// <summary>
//     // /// 订阅鼠标左键点击
//     // /// </summary>
//     // /// <param name="action"></param>
//     // public void AddListenerClickLeft(Action<Vector3> action)
//     // {
//     //     if (_actionOnClickLeft == null)
//     //     {
//     //         _actionOnClickLeft = new List<Action<Vector3>>();
//     //     }
//     //
//     //     _actionOnClickLeft.Add(action);
//     // }
//     //
//     // /// <summary>
//     // /// 取消订阅
//     // /// </summary>
//     // /// <param name="action"></param>
//     // public void DeleteListenerClickLeft(Action<Vector3> action)
//     // {
//     //     if (_actionOnClickLeft == null)
//     //     {
//     //         return;
//     //     }
//     //
//     //     if (_actionOnClickLeft.Contains(action))
//     //     {
//     //         _actionOnClickLeft.Remove(action);
//     //     }
//     // }
//     //
//     // /// <summary>
//     // /// 左键点击事件触发
//     // /// </summary>
//     // public void CallListenerClickLeft(Vector3 pos)
//     // {
//     //     if (_actionOnClickLeft == null)
//     //     {
//     //         return;
//     //     }
//     //
//     //     foreach (var clickLeftEvent in _actionOnClickLeft)
//     //     {
//     //         clickLeftEvent.Invoke(pos);
//     //     }
//     // }
// }