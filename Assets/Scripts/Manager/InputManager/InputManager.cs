using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseSingleton<InputManager>, IMonoManager
{
    public void OnInit()
    {
    }

    public void Update()
    {
        CheckLeftMouseDown();
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

    private static void CheckLeftMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventManager.Instance.Dispatch(EventName.ClickLeft, Input.mousePosition);
        }
    }
}