using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaUICtrl : UICtrlBase
{
    private AreaUIModel _model;
    private AreaUIView _view;

    public override void OnInit(params object[] param)
    {
        _model = new AreaUIModel();
        _view = GetComponent<AreaUIView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        _model.OnOpen();
        _view.OpenWindow();
        CheckConfirmBtnState();
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
        _view.returnBtn.onClick.AddListener(ReturnBtnOnClick);
        _view.confirmBtn.onClick.AddListener(ConfirmBtnOnClick);
        _view.levelBtn1.onClick.AddListener(LevelBtn1OnClick);
        _view.levelBtn2.onClick.AddListener(LevelBtn2OnClick);
    }

    /// <summary>
    /// 返回按钮按下
    /// </summary>
    private void ReturnBtnOnClick()
    {
        UIManager.Instance.OpenWindow("Home");
        UIManager.Instance.CloseWindow("Area");
    }

    /// <summary>
    /// 确认按钮按下
    /// </summary>
    private void ConfirmBtnOnClick()
    {
        if (_model.level == null)
        {
            return;
        }

        UIManager.Instance.OpenWindow("Level", _model.level,_model.jsPath);
        UIManager.Instance.CloseWindow("Area");
    }

    /// <summary>
    /// 关卡1点击
    /// </summary>
    private void LevelBtn1OnClick()
    {
        ShowMap("BackGround_01.png");
        _model.level = "Level01Map.prefab";
        _model.jsPath = "level01.json";
        CheckConfirmBtnState();
    }

    /// <summary>
    /// 关卡2点击
    /// </summary>
    private void LevelBtn2OnClick()
    {
        ShowMap("BackGround_02.jpg");
        _model.level = "Level02Map.prefab";
        _model.jsPath = "level02.json";
        CheckConfirmBtnState();
    }

    /// <summary>
    /// 检查确认按钮状态
    /// </summary>
    private void CheckConfirmBtnState()
    {
        if (_model.level == null)
        {
            _view.ConfirmBtnDisable();
            return;
        }

        _view.ConfirmBtnEnable();
    }

    /// <summary>
    /// 缩略地图展示
    /// </summary>
    /// <param name="mapName"></param>
    private void ShowMap(string mapName)
    {
        var map = _view.GetChildGameObject("Map");
        var image = map.GetComponent<Image>();

        image.sprite = AssetManager.Instance.GetGameResource<Sprite>(mapName);
        image.color = Color.white;
    }
}