using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICtrl : UICtrlBase
{
    private HomeUIView _view;
    private HomeUIModel _model;

    public override void OnInit(params object[] param)
    {
        _model = new HomeUIModel();
        _view = GetComponent<HomeUIView>();
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
    }

    public override void BindEvent()
    {
        _view.startBtn.onClick.AddListener(StartBtnOnClick);
        _view.settingBtn.onClick.AddListener(SettingBtnOnclick);
    }

    private void StartBtnOnClick()
    {
        UIManager.Instance.OpenWindow("Area");
        UIManager.Instance.CloseWindow("Home");
    }

    private void SettingBtnOnclick()
    {
        Debug.Log("SettingBtnOnclick");
    }
}