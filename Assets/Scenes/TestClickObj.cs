using System;
using UnityEngine;

public class TestClickObj : MonoBehaviour
{
    private float _clickTimer;
    private Action<Vector3> _onClickAction;
    

    private void OnMouseDown()
    {
        _clickTimer = Time.time;
    }

    private void OnMouseUp()
    {
        if (Time.time - _clickTimer < 0.1f)
        {
            _onClickAction?.Invoke(Input.mousePosition);
        }
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit(Action<Vector3> clickAction)
    {
        _onClickAction = clickAction;
    }
}