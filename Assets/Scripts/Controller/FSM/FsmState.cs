using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FsmState
{
    void OnEnter();

    void OnUpdate();

    void OnExit();

}
