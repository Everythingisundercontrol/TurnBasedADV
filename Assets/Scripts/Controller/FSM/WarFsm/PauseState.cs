using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.PauseOnEnter();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.PauseOnExit();
    }
}