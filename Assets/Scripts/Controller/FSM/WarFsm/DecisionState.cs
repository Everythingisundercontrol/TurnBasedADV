using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.DecisionOnEnter();
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        WarManager.Instance.DecisionOnExit();
    }
}
