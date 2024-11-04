using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    }

    public override void BindEvent()
    {
        _view.returnBtn.onClick.AddListener(ReturnBtnOnClick);
        _view.startBtn.onClick.AddListener(StartBtnOnClick);
    }
    
    /// <summary>
    /// 开始按钮是否可交互
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
    /// 返回按钮按下事件，清空数据并且删除point和line
    /// </summary>
    private void ReturnBtnOnClick()
    {
        _model.OnClose();
        GameManager.Instance.StartCoroutine(LoadSceneAndOpenWindow());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 加载新场景并且切换页面
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSceneAndOpenWindow()
    {
        yield return SceneManager.Instance.ChangeSceneAsync("SampleScene");
        UIManager.Instance.OpenWindow("AreaView.prefab");
        UIManager.Instance.CloseWindow("LevelView.prefab");
    }

    private void StartBtnOnClick()
    {
        //todo: 在有编队之前为不可点击状态，warFsm进入TurnInitState
    }


}