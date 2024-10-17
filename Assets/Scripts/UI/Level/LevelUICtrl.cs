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
            LoadPointModel("level01.json");
            InitData();
            InitGameObject();
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
    /// 动态数据加载
    /// </summary>
    private void LoadPointModel(string path)
    {
        var points = AssetManager.Instance.LoadJsonFile<PointList>(path).Points;

        foreach (var point in points)
        {
            _model.PointModels.Add(point.pointID, point);
        }
    }

    /// <summary>
    /// 从json文件中加载点位
    /// </summary>
    private void LoadPointData()
    {
        var pointDatas = AssetManager.Instance.LoadJsonFile<PointDataList>("pointData.json").PointDatas;

        if (pointDatas == null)
        {
            return;
        }

        foreach (var pointData in pointDatas)
        {
            if (!_model.PointModels.ContainsKey(pointData.pointID))
            {
                continue;
            }

            if (_model.PointData.ContainsKey(pointData.pointID))
            {
                continue;
            }

            _model.PointData.Add(pointData.pointID, pointData);
        }
    }

    /// <summary>
    /// 从json文件中加载单位  //todo：拆
    /// </summary>
    private void LoadUnit()
    {
        var units = AssetManager.Instance.LoadJsonFile<UnitList>("unitData.json").Units;
        if (units == null)
        {
            return;
        }

        foreach (var unit in units)
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
    /// 从json文件中加载事件    //todo:
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
    /// 游戏物体初始化
    /// </summary>
    private void InitGameObject()
    {
        InitPoint();
        InitLine();
        InitUnit();
        InitEvent();
    }

    /// <summary>
    /// 点位初始化
    /// </summary>
    private void InitPoint()
    {
        foreach (var pointModel in _model.PointModels)
        {
            CreatePoints(_model.PointData[pointModel.Key]);
        }
    }

    /// <summary>
    /// 连接线初始化
    /// </summary>
    private void InitLine()
    {
        foreach (var pointModel in _model.PointModels)
        {
            CheckNextPoint(_model.PointData[pointModel.Key]);
        }
    }

    private void InitUnit()
    {
        foreach (var pointModel in _model.PointModels)
        {
            if (pointModel.Value.unitID == null)
            {
                continue;
            }

            CreateUnit(pointModel.Key);
        }
    }

    private void InitEvent()
    {
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
            CreateCntPoint(pointPosition, pointID);
            return;
        }

        if (_model.PointModels[inputPointData.pointID].eventID != "")
        {
            CreateEventPoint(pointPosition, pointID);
            return;
        }

        _view.CreatePointsObj(pointPosition, pointID, _view.GameObjectPrefabs["Point"]);
    }

    /// <summary>
    /// 创建可建立队伍点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateCntPoint(Vector3 pointPosition, string pointID)
    {
        var pointPrefab = _view.GameObjectPrefabs["CNTPoint"];
        _view.CreatePointsObj(pointPosition, pointID, pointPrefab);

        var pointGameObject = GETPointViewByID(pointID);
        AddCntPointEvent(pointGameObject);
    }

    /// <summary>
    /// 创建事件点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateEventPoint(Vector3 pointPosition, string pointID)
    {
        var pointPrefab = _view.GameObjectPrefabs["EventPoint"];
        _view.CreatePointsObj(pointPosition, pointID, pointPrefab);
    }


    /// <summary>
    /// 检查NextPoint是否为空，不为空则连接
    /// </summary>
    private void CheckNextPoint(PointData pointData)
    {
        if (pointData.nextPoints == null)
        {
            return;
        }

        ConnectPoints(pointData);
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
    /// 创建地图单位
    /// </summary>
    private void CreateUnit(string pointID)
    {
        var unitID = _model.PointModels[pointID].unitID;
        if (_model.UnitData[unitID].ifKindness)
        {
            CreateTeam(pointID);
        }

        CreateEnemy(pointID);
    }

    /// <summary>
    /// 创建敌人
    /// </summary>
    private void CreateEnemy(string pointID)
    {
        //model
        var unitID = _model.PointModels[pointID].unitID;
        var unit = _model.UnitData[unitID];

        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        _model.EnemyModels.Add(pointID, unit);

        //view
        var point = _model.PointData[pointID];
        var position = new Vector3();
        position.x = point.positionX;
        position.y = point.positionY;
        position.z = 0;
        
        // _view.CreateUnitObj(pointID,position);
    }

    /// <summary>
    /// 创建我方队伍
    /// </summary>
    private void CreateTeam(string pointID)
    {
        //model
        var unitID = _model.PointModels[pointID].unitID;
        var unit = _model.UnitData[unitID];

        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        _model.EnemyModels.Add(pointID, unit);

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

        Debug.LogError("getPointByID : " + id + "is null");
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
        var points = new PointList
        {
            Points = new List<Point>
            {
                new Point
                {
                    pointID = "002",
                    eventID = null,
                    unitID = null
                },
                new Point
                {
                    pointID = "003",
                    eventID = null,
                    unitID = null
                }
            }
        };

        var pointData = new PointDataList
        {
            PointDatas = new List<PointData>
            {
                new PointData
                {
                    pointID = "002",
                    positionX = 0,
                    positionY = 0,
                    nextPoints = new List<string>
                    {
                        "003"
                    }
                },
                new PointData
                {
                    pointID = "003",
                    positionX = 1,
                    positionY = 0,
                    nextPoints = new List<string>
                    {
                    }
                }
            }
        };

        var units = new UnitList
        {
            Units = new List<Unit>
            {
                new Unit
                {
                    unitID = "unit_01",
                    PointID = new List<string>
                    {
                        "002"
                    },
                    MemberID = new List<string>
                    {
                        "member_01"
                    },
                    teamAp = 3,
                    ifKindness = false
                },
                new Unit
                {
                    unitID = "unit_02",
                    PointID = new List<string>
                    {
                        "003"
                    },
                    MemberID = new List<string>
                    {
                        "member_02"
                    },
                    teamAp = 2,
                    ifKindness = true
                }
            }
        };

        var members = new MemberList
        {
            Members = new List<Member>
            {
                new Member
                {
                    memberID = "member_01",
                    prefabPath = "member_01.gif",
                    name = "test01",
                    HP = 100,
                    MP = 100
                },
                new Member
                {
                    memberID = "member_02",
                    prefabPath = "member_02.gif",
                    name = "test02",
                    HP = 90,
                    MP = 110
                }
            }
        };

        var js1 = JsonUtility.ToJson(pointData);
        var js2 = JsonUtility.ToJson(units);
        var js3 = JsonUtility.ToJson(members);
        var path1 = Path.Combine(Application.persistentDataPath, "pointData.json");
        var path2 = Path.Combine(Application.persistentDataPath, "unitData.json");
        var path3 = Path.Combine(Application.persistentDataPath, "memberData.json");

        var js4 = JsonUtility.ToJson(points);
        var path4 = Path.Combine(Application.persistentDataPath, "level01.json");
        File.WriteAllText(path4, js4);

        File.WriteAllText(path1, js1);
        File.WriteAllText(path2, js2);
        File.WriteAllText(path3, js3);
        // Debug.Log("file save to: " + path);
    }
}