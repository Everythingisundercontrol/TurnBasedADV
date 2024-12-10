using System;
using UnityEngine;

public class EndGameState : FsmState
{
    public void OnEnter()
    {
        WarManager.Instance.EndGameOnEnter();
        //todo:根据游戏状态决定结束游戏的对应事件
        // switch (WarManager.Instance.Model.GameState)
        // {
        //     case GameStateEnum.Win:
        //         return;
        //     case GameStateEnum.Fail:
        //         return;
        //     case GameStateEnum.Default:
        //         return;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        WarManager.Instance.EndGameOnExit();
    }
}