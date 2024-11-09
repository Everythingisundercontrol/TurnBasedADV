﻿using System.Collections;
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
    /// 展示Tp
    /// </summary>
    public void ShowTeamPoint(int tp)
    {
        _view.SetTp(tp);
    }

    /// <summary>
    /// 展示当前聚焦队伍信息
    /// </summary>
    public void ShowFocosOnUnit(string spritePath)
    {
        var sp = AssetManager.Instance.LoadAsset<Sprite>(spritePath);
        _view.SetLeaderPic(sp);
    }

    /// <summary>
    /// ui页面进入decision动作
    /// </summary>
    public void DecisionOnEnterUI()
    {
        
    }

    /// <summary>
    /// 返回按钮按下事件
    /// </summary>
    private void ReturnBtnOnClick()
    {
        _model.OnClose();
        GameManager.Instance.StartCoroutine(QuitWar());
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

    /// <summary>
    /// 正式开始游戏
    /// </summary>
    private void StartBtnOnClick()
    {
        //todo: 在有编队之前为不可点击状态，warFsm进入TurnInitState
        WarManager.Instance.LevelStartBtnOnClickEvent();
        _view.ShowTeamInfo(true);
        _view.startBtnObj.SetActive(false);
        _view.TurnEndBtnObj.SetActive(true);
    }
}