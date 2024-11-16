using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PointSelectCtrl : UICtrlBase
{
    public Camera mainCamara;
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
        ButtonCreate();
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
        _view.Mask.onClick.AddListener(MaskOnClick);
    }

    /// <summary>
    /// 创建button
    /// </summary>
    private void ButtonCreate()
    {
        mainCamara = Camera.main;
        _view.MapPointDic = new Dictionary<string, GameObject>();
        var prefab = AssetManager.Instance.LoadAsset<GameObject>("MapPointBtn.prefab");
        foreach (var pointID in WarManager.Instance.Model.PointModels.Keys)
        {
            var position = GetPointPosition(pointID);
            var button = Instantiate(prefab, transform);
            button.transform.position = position;
            button.name = pointID;
            button.GetComponent<Button>().onClick.AddListener(() => PointButtonOnClick(button.GetComponent<Button>()));
        }
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    private void PointButtonOnClick(Button button)
    {
        Debug.Log(button.name);
        WarManager.Instance.TeamMove(button.name);
        WarManager.Instance.MoveEventEnd();
        UIManager.Instance.CloseWindow("PointSelect.prefab");
    }

    /// <summary>
    /// 
    /// </summary>
    private void MaskOnClick()
    {
        WarManager.Instance.MoveEventEnd();
        UIManager.Instance.CloseWindow("PointSelect.prefab");
    }
}