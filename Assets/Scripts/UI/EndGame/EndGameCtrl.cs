using UnityEngine;
using UnityEngine.UI;

public class EndGameCtrl : UICtrlBase
{
    private EndGameModel _model;
    private EndGameView _view;

    public override void OnInit(params object[] param)
    {
        _model = new EndGameModel();
        _view = GetComponent<EndGameView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        _model.OnOpen();
        ShowInfo();
        _view.OpenWindow();
    }

    public override void CloseRoot()
    {
        _view.CloseWindow();
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {
        _view.ReturnBtnObj.GetComponent<Button>().onClick.AddListener(ReturnBtnOnClick);
    }

    /// <summary>
    /// 展示游戏结局信息
    /// </summary>
    private void ShowInfo()
    {
        Debug.Log("ShowInfo " + _model.GameOutComeText +" "+ WarManager.Instance.Model.TeamModels.Count+" "+WarManager.Instance.Model.EnemyModels.Count);
        _view.ShowGameOutCome(_model.GameOutComeText);
    }

    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
    private void ReturnBtnOnClick()
    {
        UIManager.Instance.OpenWindow("AreaView.prefab");
        UIManager.Instance.CloseWindow("EndGameView.prefab");
    }
}