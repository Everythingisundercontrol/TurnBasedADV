using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.BattleOnEnter();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.BattleOnExit();
    }
}