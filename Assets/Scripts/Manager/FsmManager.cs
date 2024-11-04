using System.Collections.Generic;

public class FsmManager : BaseSingleton<FsmManager>, IMonoManager
{
    private Dictionary<FsmEnum, BaseFSM> _fsm;

    public void OnInit()
    {
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
    public void setFsmState(FsmEnum fsmEnum, FsmStateEnum fsmStateEnum)
    {
        _fsm[fsmEnum].ChangeFsmState(fsmStateEnum);
    }

    /// <summary>
    /// warFsm初始化
    /// </summary>
    private void WarFsmOnInit()
    {
        _fsm = new Dictionary<FsmEnum, BaseFSM>();
        var warFsm = new WarFsm
        {
            fsmName = "warFsm"
        };
        var warFsmDic = new Dictionary<FsmStateEnum, FsmState>();

        warFsmDic.Add(FsmStateEnum.War_SetUpState, new SetUpState());
        warFsmDic.Add(FsmStateEnum.War_TurnInitState, new TurnInitState());
        warFsmDic.Add(FsmStateEnum.War_DecisionState, new DecisionState());
        warFsmDic.Add(FsmStateEnum.War_EndTurnState, new EndTurnState());
        warFsmDic.Add(FsmStateEnum.War_EndGameState, new EndGameState());
        warFsmDic.Add(FsmStateEnum.War_PauseState, new PauseState());
        warFsmDic.Add(FsmStateEnum.War_BattleState, new BattleState());

        warFsm.SetFsm(warFsmDic);
        _fsm.Add(FsmEnum.warFsm, warFsm);
    }
}