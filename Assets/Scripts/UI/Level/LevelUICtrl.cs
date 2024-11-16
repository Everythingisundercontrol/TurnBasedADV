using System.Collections;
using UnityEngine;

public class LevelUICtrl : UICtrlBase
{
    private LevelUIModel _model;
    private LevelUIView _view;

    public override void OnInit(params object[] param)
    {
        _model = new LevelUIModel();
        _view = GetComponent<LevelUIView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        _model.OnOpen();
        _view.OpenWindow();
    }

    public override void CloseRoot()
    {
        _view.CloseWindow();
    }

    public override void OnClear()
    {
        _view.tp.text = "[TP: Nan ]";
        _view.teamInfo.SetActive(false);
        _view.startBtnObj.SetActive(true);
    }

    public override void BindEvent()
    {
        _view.returnBtn.onClick.AddListener(ReturnBtnOnClick);
        _view.startBtn.onClick.AddListener(StartBtnOnClick);
    }

    /// <summary>
    /// 检查开始按钮是否可交互
    /// </summary>
    public void CheckStartBtnState()
    {
        if (WarManager.Instance.Model.StartAble)
        {
            _view.StartBtnEnable();
            return;
        }

        _view.StartBtnDisable();
    }

    /// <summary>
    /// 展示Tp
    /// </summary>
    public void ShowTeamPoint(int tp)
    {
        _view.SetTp(tp);
    }

    /// <summary>
    /// 展示当前聚焦队伍信息  //todo:增加数据
    /// </summary>
    public void ShowFocosOnUnit()
    {
        var focosOnPoint = WarManager.Instance.Model.PointModels[WarManager.Instance.Model.FocosOnPointID];
        var focosOnUnitID = focosOnPoint.unitID;
        var leaderMemberID = WarManager.Instance.Model.TeamModels[focosOnUnitID].MemberID[0];
        var path = WarManager.Instance.Model.MemberData[leaderMemberID].prefabPath;
        var sp = AssetManager.Instance.LoadAsset<Sprite>(path);
        _view.SetLeaderPic(sp);
    }

    /// <summary>
    /// ui页面进入decision动作    //todo:UI事件绑定，比如加buff
    /// </summary>
    public void DecisionOnEnterUI()
    {
        System.Threading.Thread.Sleep(1000);
        _view.ShowTeamInfo(true);
        _view.TurnEndBtnObj.SetActive(true);
        _view.TurnEndBtn.onClick.AddListener(TurnEndBtnOnClick);
        _view.MoveBtn.onClick.AddListener(MoveBtnOnClick);
    }

    /// <summary>
    /// ui页面退出decision动作
    /// </summary>
    public void DecisionOnExitUI()
    {
        _view.ShowTeamInfo(false);
        _view.TurnEndBtnObj.SetActive(false);
        _view.TurnEndBtn.onClick.RemoveListener(TurnEndBtnOnClick);
        _view.MoveBtn.onClick.RemoveListener(MoveBtnOnClick);
    }

    /// <summary>
    /// 
    /// </summary>
    public void MoveEventEnd()
    {
        _view.MoveBtn.interactable = true;
    }

    /// <summary>
    /// 返回按钮按下事件
    /// </summary>
    private void ReturnBtnOnClick()
    {
        _model.OnClose();
        GameManager.Instance.StartCoroutine(QuitWar());
    }

    /// <summary>
    /// 开始按钮按下事件，正式开始游戏
    /// </summary>
    private void StartBtnOnClick()
    {
        //todo: 在有编队之前为不可点击状态，warFsm进入TurnInitState
        WarManager.Instance.LevelStartBtnOnClickEvent();

        //点击后，开始按钮关闭，回合结束按钮开启
        _view.startBtnObj.SetActive(false);
    }

    /// <summary>
    /// 回合结束按钮按下事件
    /// </summary>
    private void TurnEndBtnOnClick()
    {
        
        WarManager.Instance.LevelTurnEndBtnOnClickEvent();
    }

    /// <summary>
    /// 移动按钮按下事件
    /// </summary>
    private void MoveBtnOnClick()
    {
        Debug.Log("MoveBtnOnClick");
        UIManager.Instance.OpenWindow("PointSelect.prefab");
        _view.MoveBtn.interactable = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 加载新场景并且退出war
    /// </summary>
    /// <returns></returns>
    private IEnumerator QuitWar()
    {
        yield return SceneManager.Instance.ChangeSceneAsync("Home");
        WarManager.Instance.OnQuit();
        UIManager.Instance.OpenWindow("AreaView.prefab");
    }
}