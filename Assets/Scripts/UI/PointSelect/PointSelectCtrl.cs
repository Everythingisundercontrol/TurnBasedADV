using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PointSelectCtrl : UICtrlBase
{
    private PointSelectView _view;
    private PointSelectModel _model;

    public override void OnInit(params object[] param)
    {
        _model = new PointSelectModel();
        _view = GetComponent<PointSelectView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        ButtonCreate();
        _model.OnOpen();
        _view.OpenWindow();
    }

    public override void CloseRoot()
    {
        PointButtonOnClickRemove();
        _view.CloseWindow();
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {
        _view.Mask.onClick.AddListener(MaskOnClick);
    }

    private void PointButtonOnClickRemove()
    {
        foreach (var buttonObj in _view.MapPointDic.Values)
        {
            buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 对应点位位置上创建button，并挂载事件
    /// </summary>
    private void ButtonCreate()
    {
        _view.MapPointDic = new Dictionary<string, GameObject>();
        var prefab = AssetManager.Instance.LoadAsset<GameObject>("MapPointBtn.prefab");
        foreach (var pointID in WarManager.Instance.Model.PointModels.Keys)
        {
            var position = GetPointPosition(pointID);
            var button = Instantiate(prefab, transform);
            button.transform.position = position;
            Debug.Log("ButtCreate : " + pointID + " " + button.transform.position.x);
            button.name = pointID;
            button.GetComponent<Button>().onClick.AddListener(() => PointButtonOnClick(button.name));
            _view.MapPointDic.Add(button.name, button);
        }
    }

    /// <summary>
    /// 获取点位位置
    /// </summary>
    /// <param name="pointID"></param>
    /// <returns></returns>
    private Vector3 GetPointPosition(string pointID)
    {
        if (!WarManager.Instance.Model.PointData.ContainsKey(pointID))
        {
            Debug.LogError("GetPointPosition : PointID cannot find");
        }

        var X = WarManager.Instance.Model.PointData[pointID].positionX;
        var Y = WarManager.Instance.Model.PointData[pointID].positionY;

        var worldPos = new Vector3(X, Y, 100);
        // var screenPos = mainCamara.WorldToScreenPoint(worldPos);
        return worldPos;
    }

    /// <summary>
    ///  点位点击事件
    /// </summary>
    private void PointButtonOnClick(string button)
    {
        Debug.Log("PointButtonOnClick");
        WarManager.Instance.TeamMove(button);
        WarManager.Instance.MoveEventEnd();

        UIManager.Instance.CloseWindow("PointSelect.prefab");
    }

    /// <summary>
    /// 点击非点位事件，退出点位选择
    /// </summary>
    private void MaskOnClick()
    {
        Debug.Log("MaskOnClick");
        WarManager.Instance.MoveEventEnd();
        UIManager.Instance.CloseWindow("PointSelect.prefab");
    }
}