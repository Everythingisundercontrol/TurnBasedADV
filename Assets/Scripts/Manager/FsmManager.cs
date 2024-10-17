using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmManager : MonoSingleton<FsmManager>, IMonoManager
{
    private Dictionary<FsmEnum, BaseFSM> _fsm;

    public void OnInit()
    {
        Debug.Log("FsmManager.OnInit");
        WarFsmOnInit();
    }

    public void Update()
    {
        foreach (var baseFsm in _fsm)
        {
            baseFsm.Value.OnUpdate();
        }
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
    /// 外部设置FSM的状态
    /// </summary>
    /// <param name="fsmEnum"></param>
    /// <param name="fsmStateEnum"></param>
    public void setFsmState(FsmEnum fsmEnum,FsmStateEnum fsmStateEnum)
    {
        _fsm[fsmEnum].ChangeFsmState(fsmStateEnum);
    }
    
    /// <summary>
    /// warFsm初始化
    /// </summary>
    private void WarFsmOnInit()
    {
        _fsm = new Dictionary<FsmEnum, BaseFSM>();
        var warFsm = new WarFsm();
        var warFsmDic = new Dictionary<FsmStateEnum, FsmState>();

        var setUpState = new SetUpState();
        var turnInitState = new TurnInitState();
        warFsmDic.Add(FsmStateEnum.War_SetUpState, setUpState);
        warFsmDic.Add(FsmStateEnum.War_TurnInitState, turnInitState);

        warFsm.SetFsm(warFsmDic);
        _fsm.Add(FsmEnum.warFsm, warFsm);
        // fsm[FsmEnum.warFsm].ChangeFsmState(FsmStateEnum.War_SetUpState);
    }
    
}