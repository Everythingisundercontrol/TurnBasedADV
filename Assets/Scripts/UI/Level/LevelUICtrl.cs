using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LevelUICtrl : UICtrlBase
{
    private LevelUIModel _model;
    private LevelUIView _view;

    public override void OnInit(params object[] param)
    {
        _model = new LevelUIModel();
        _view = GetComponent<LevelUIView>();
        _view.InitDic();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        if (param != null && param.Length > 0)
        {
            LoadMap((string) param[0]);
            LoadPointModel();
            InitData();
        }

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
    /// 返回按钮按下事件，清空数据并且删除point和line
    /// </summary>
    private void ReturnBtnOnClick()
    {
        _model.OnClose();
        _view.OnClose();
        UIManager.Instance.OpenWindow("Area");
        UIManager.Instance.CloseWindow("Level");
    }

    private void StartBtnOnClick()
    {
        InputJsonTest();

        //todo: 在有编队之前为不可点击状态，warFsm进入TurnInitState
    }

    private void CNTPointBtnOnClick()
    {
        //todo: 新页面，编队与携带物资
    }

    /// <summary>
    /// 静态数据读入
    /// </summary>
    private void InitData()
    {
        LoadPointData();
        LoadUnit();
        LoadEvent();
    }

    /// <summary>
    /// 读取
    /// </summary>
    private void LoadPointModel()
    {
        // var js = AssetManager.Instance.ReadJsonFileToString(path + ".json");
        // var records = JsonUtility.FromJson<PointList>(js);
        // if ()
        // {
        // }

        _model.PointModels.Add("002", null);
    }

    /// <summary>
    /// 从json文件中加载点位
    /// </summary>
    private void LoadPointData()
    {
        var js = AssetManager.Instance.ReadJsonFileToString("pointData.json");
        var records = JsonUtility.FromJson<PointDataList>(js);

        foreach (var pointData in records.PointDatas)
        {
            if (!_model.PointModels.ContainsKey(pointData.pointID))
            {
                continue;
            }

            _model.PointData.Add(pointData.pointID, pointData);
        }
    }

    /// <summary>
    /// 从json文件中加载单位
    /// </summary>
    private void LoadUnit()
    {
        var js = AssetManager.Instance.ReadJsonFileToString("unitData.json");
        var records = JsonUtility.FromJson<UnitList>(js);

        foreach (var unit in records.Units)
        {
            foreach (var pointID in _model.PointModels.Keys)
            {
                if (_model.UnitData.ContainsKey(pointID))
                {
                    continue;
                }

                if (!unit.PointID.Contains(pointID))
                {
                    continue;
                }

                _model.UnitData.Add(unit.unitID, unit);
                _model.PointModels[pointID].unitID = unit.unitID;
            }
        }
    }

    /// <summary>
    /// 从json文件中加载事件
    /// </summary>
    private void LoadEvent()
    {
        var js = AssetManager.Instance.ReadJsonFileToString("eventData.json");
        var records = JsonUtility.FromJson<EventList>(js);
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap(string mapName)
    {
        var mapPrefab = AssetManager.Instance.GetGameResource<GameObject>(mapName);
        if (mapPrefab)
        {
            _view.InitMap(mapPrefab);
        }
    }


    /// <summary>
    /// 创建point
    /// </summary>
    private void CreatePoints(PointData inputPointData)
    {
        var pointPosition = new Vector3(inputPointData.positionX, inputPointData.positionY, 0);
        var pointID = inputPointData.pointID;

        if (inputPointData.canNewTeam)
        {
            var pointPrefab = _view.GameObjectPrefabs["CNTPoint"];
            _view.CreatePointsObj(pointPosition, pointID, pointPrefab);

            var pointGameObject = GETPointViewByID(pointID);
            AddCntPointEvent(pointGameObject);
            return;
        }

        if (_model.PointModels[inputPointData.pointID].eventID != "")
        {
            var pointPrefab = _view.GameObjectPrefabs["EventPoint"];
            _view.CreatePointsObj(pointPosition, pointID, pointPrefab);
            return;
        }

        _view.CreatePointsObj(pointPosition, pointID, _view.GameObjectPrefabs["Point"]);

        CreateUnit(pointID);
    }

    /// <summary>
    /// 检查NextPoint是否为空，不为空则连接
    /// </summary>
    private void CheckNextPoint(IEnumerable<PointData> pointDatas)
    {
        foreach (var point in pointDatas)
        {
            if (point.nextPoints == null)
            {
                continue;
            }

            ConnectPoints(point);
        }
    }

    /// <summary>
    /// 连接当前点和下一点
    /// </summary>
    /// <param name="point"></param>
    private void ConnectPoints(PointData pointData)
    {
        var lineContainer = GameObject.Find("Lines");
        if (!lineContainer)
        {
            return;
        }

        foreach (var nextPointID in pointData.nextPoints)
        {
            var startPoint = GETPointDataByID(pointData.pointID);
            var endPoint = GETPointDataByID(nextPointID);
            var start = new Vector3(startPoint.positionX, startPoint.positionY, 0);
            var end = new Vector3(endPoint.positionX, endPoint.positionY, 0);
            _view.CreateLineObj(start, end, lineContainer);
        }
    }

    /// <summary>
    /// 创建地图单位      ///暂时把创建地图单位绑在创建点位函数中
    /// </summary>
    private void CreateUnit(string pointID)
    {
        // if (inputPoint.unitID == null)
        // {
        //     return;
        // }
    }

    /// <summary>
    /// 创建敌人
    /// </summary>
    private void CreateEnemy(Point inputPoint)
    {
        //model
        // _model.Enemy.Add(inputPoint.pointID,inputPoint.unit);

        //view
    }

    /// <summary>
    /// 创建我方队伍
    /// </summary>
    private void CreateTeam(Point inputPoint)
    {
        //model
        // _model.Team.Add(inputPoint.pointID,inputPoint.unit);

        //view
    }


    /// <summary>
    /// 通过ID获得点位model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private PointData GETPointDataByID(string id)
    {
        if (_model.PointModels.ContainsKey(id))
        {
            return _model.PointData[id];
        }

        Debug.LogError("getPointByID is null");
        return null;
    }

    /// <summary>
    /// 通过ID获得点位gameobject
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private GameObject GETPointViewByID(string id)
    {
        if (_view.PointGameObjects.ContainsKey(id))
        {
            return _view.PointGameObjects[id];
        }

        Debug.LogError("getPointByID is null");
        return null;
    }

    /// <summary>
    /// 添加CNT事件
    /// </summary>
    /// <param name="point"></param>
    private void AddCntPointEvent(GameObject point)
    {
        var pointBtn = point.GetComponent<Button>();
        if (!pointBtn)
        {
            Debug.LogError("CNTPointButton is null");
            return;
        }

        pointBtn.onClick.AddListener(CNTPointBtnOnClick);
    }

    /// <summary>
    /// 
    /// </summary>
    private void InputJsonTest()
    {
        var point1 = new Point
        {
            pointID = "002",
            eventID = "",
            unitID = ""
        };

        var pointData = new PointDataList
        {
            PointDatas = new List<PointData>
            {
                new PointData
                {
                    pointID = "002",
                    positionX = 1,
                    positionY = 0,
                    nextPoints = new List<string>
                    {
                        "003"
                    }
                }
            }
        };

        var unit1 = new Unit
        {
            unitID = "unit_01",
            MemberID = new List<string>
            {
                "member_01"
            },
            teamAp = 3,
            ifKindness = false
        };

        var member1 = new Member
        {
            memberID = "member_01",
            prefabPath = "member_01.gif",
            name = "test01",
            HP = 100,
            MP = 100
        };
        

        var js1 = JsonUtility.ToJson(pointData);
        var js2 = JsonUtility.ToJson(unit1);
        var js3 = JsonUtility.ToJson(member1);
        var path1 = Path.Combine(Application.persistentDataPath, "pointData.json");
        var path2 = Path.Combine(Application.persistentDataPath, "unitData.json");
        var path3 = Path.Combine(Application.persistentDataPath, "memberData.json");
        File.WriteAllText(path1, js1);
        File.WriteAllText(path2, js2);
        File.WriteAllText(path3, js3);
        // Debug.Log("file save to: " + path);
    }
}