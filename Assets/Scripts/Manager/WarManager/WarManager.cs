using System.Collections.Generic;
using UnityEngine;

public class WarManager : BaseSingleton<WarManager>, IMonoManager
{
    public WarModel Model;

    private List<GameObject> _lineList; //不确定有没有用
    private Dictionary<string, GameObject> _pointGameObjects;
    private Dictionary<string, GameObject> _unitGameObjects;

    private LevelUICtrl _ctrl;

    private string _level;
    private string _jsPath;

    public void OnInit(string level, string jsPath)
    {
        _level = level;
        _jsPath = jsPath;
        Model = new WarModel();
        Model.OnInit();

        _lineList = new List<GameObject>();
        _pointGameObjects = new Dictionary<string, GameObject>();
        _unitGameObjects = new Dictionary<string, GameObject>();

        _ctrl = UIManager.Instance.GetLevelUICtrl();

        FsmManager.Instance.setFsmState(FsmEnum.warFsm, FsmStateEnum.War_SetUpState);
        UIManager.Instance.OpenWindow("LevelView.prefab");
    }

    public void OnInit()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }
    
    /// <summary>
    /// SetUp初始化
    /// </summary>
    public void SetUpOnEnter()
    {
        LoadMap(_level);
        LoadPointModel(_jsPath);
        InitData();
        InitGameObject();
        BindEvent();
    }

    /// <summary>
    /// SetUp退出,把所有cntP变为普通点
    /// </summary>
    public void SetUpOnExit()
    {
        foreach (var pointID in _pointGameObjects.Keys)
        {
            if (Model.PointData[pointID].canNewTeam == false)
            {
                continue;
            }

            Model.PointData[pointID].canNewTeam = false;
            var point = AssetManager.Instance.LoadAsset<GameObject>("Point.prefab");
            ChangePoint(pointID, point);
        }
    }

    public static void LevelStartBtnOnClickEvent()
    {
        FsmManager.Instance.setFsmState(FsmEnum.warFsm, FsmStateEnum.War_TurnInitState);
    }

    public void LevelReturnBtnOnClickEvent()
    {
        FsmManager.Instance.setFsmState(FsmEnum.warFsm, FsmStateEnum.War_EndGameState);
    }

    /// <summary>
    /// 事件绑定
    /// </summary>
    private void BindEvent()
    {
        EventManager.Instance.AddListener<Vector3>(EventName.ClickLeft, ClickLeft);
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap(string mapName)
    {
        var mapPrefab = AssetManager.Instance.LoadAsset<GameObject>(mapName);
        if (!mapPrefab)
        {
            return;
        }

        var mapContainer = GameObject.Find("Map");
        if (!mapContainer)
        {
            return;
        }

        Object.Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, mapContainer.transform);
    }

    /// <summary>
    /// 加载动态数据
    /// </summary>
    private void LoadPointModel(string path)
    {
        var points = AssetManager.LoadJsonFile<PointList>(path).Points;

        foreach (var point in points)
        {
            Model.PointModels.Add(point.pointID, point);
        }
    }

    /// <summary>
    /// 静态数据读入
    /// </summary>
    private void InitData()
    {
        LoadPointData();
        LoadUnit();
        LoadMember();
        LoadEvent();
    }

    /// <summary>
    /// 游戏物体初始化
    /// </summary>
    private void InitGameObject()
    {
        InitPoint();
        InitLine();
        InitUnit();
        // InitEvent();
    }

    /// <summary>
    /// 从json文件中加载点位
    /// </summary>
    private void LoadPointData()
    {
        var pointDatas = AssetManager.LoadJsonFile<PointDataList>("pointData.json").PointDatas;

        if (pointDatas == null)
        {
            return;
        }

        foreach (var pointData in pointDatas)
        {
            if (!Model.PointModels.ContainsKey(pointData.pointID))
            {
                continue;
            }

            if (Model.PointData.ContainsKey(pointData.pointID))
            {
                continue;
            }

            Model.PointData.Add(pointData.pointID, pointData);
        }
    }

    /// <summary>
    /// 从json文件中加载单位  //todo：拆
    /// </summary>
    private void LoadUnit()
    {
        var units = AssetManager.LoadJsonFile<UnitList>("unitData.json").Units;
        if (units == null)
        {
            return;
        }

        foreach (var unit in units)
        {
            foreach (var pointID in Model.PointModels.Keys)
            {
                if (Model.UnitData.ContainsKey(pointID))
                {
                    continue;
                }

                if (!unit.PointID.Contains(pointID))
                {
                    continue;
                }

                Model.UnitData.Add(unit.unitID, unit);
                Model.PointModels[pointID].unitID = unit.unitID;
            }
        }
    }

    /// <summary>
    /// 从json文件中加载角色
    /// </summary>
    private void LoadMember()
    {
        var members = AssetManager.LoadJsonFile<MemberList>("memberData.json").Members;
        if (members == null)
        {
            return;
        }

        //todo:待优化，有筛选地填入
        foreach (var member in members)
        {
            Model.MemberData.Add(member.memberID, member);
        }
    }

    /// <summary>
    /// 从json文件中加载事件    //todo:拆
    /// </summary>
    private void LoadEvent()
    {
        var events = AssetManager.LoadJsonFile<EventList>("eventData.json").Events;
        if (events == null)
        {
            return;
        }

        foreach (var eEvent in events)
        {
            foreach (var pointID in Model.PointModels.Keys)
            {
                if (Model.EventData.ContainsKey(pointID)) //data已经有了就不再录入
                {
                    continue;
                }

                if (!eEvent.pointID.Contains(pointID)) //这个事件没有这个点位就跳过
                {
                    continue;
                }

                Model.EventData.Add(eEvent.eventID, eEvent);
                Model.PointModels[pointID].eventID = eEvent.eventID;
            }
        }
    }

    /// <summary>
    /// 点位初始化
    /// </summary>
    private void InitPoint()
    {
        foreach (var pointModel in Model.PointModels)
        {
            CreatePoints(Model.PointData[pointModel.Key]);
        }
    }

    /// <summary>
    /// 连接线初始化
    /// </summary>
    private void InitLine()
    {
        foreach (var pointModel in Model.PointModels)
        {
            CheckNextPoint(Model.PointData[pointModel.Key]);
        }
    }

    /// <summary>
    /// 单位初始化
    /// </summary>
    private void InitUnit()
    {
        foreach (var pointModel in Model.PointModels)
        {
            if (string.IsNullOrEmpty(pointModel.Value.unitID))
            {
                continue;
            }

            CreateUnit(pointModel.Key);
        }
    }

    /// <summary>
    /// 事件初始化//暂时不做，没需求
    /// </summary>
    private void InitEvent()
    {
        // foreach (var pointModel in _model.PointModels)
        // {
        //     if (string.IsNullOrEmpty(pointModel.Value.eventID))
        //     {
        //         continue;
        //     }
        // }
    }

    /// //////////////////////////////////
    /// point
    /// //////////////////////////////////
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

        if (Model.PointModels[inputPointData.pointID].eventID != "")
        {
            CreateEventPoint(pointPosition, pointID);
            return;
        }

        var point = AssetManager.Instance.LoadAsset<GameObject>("Point.prefab");
        CreatePointsObj(pointPosition, pointID, point);
    }

    /// <summary>
    /// 创建可建立队伍点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateCntPoint(Vector3 pointPosition, string pointID)
    {
        var point = AssetManager.Instance.LoadAsset<GameObject>("CNTPoint.prefab");
        CreatePointsObj(pointPosition, pointID, point);
    }

    /// <summary>
    /// 创建事件点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateEventPoint(Vector3 pointPosition, string pointID)
    {
        var point = AssetManager.Instance.LoadAsset<GameObject>("EventPoint.prefab");
        CreatePointsObj(pointPosition, pointID, point);
    }

    /// <summary>
    /// 创建point的obj
    /// </summary>
    private void CreatePointsObj(Vector3 inputVector3, string pointID, GameObject inputGameObject)
    {
        var pointContainer = GameObject.Find("Points");
        if (!pointContainer)
        {
            return;
        }

        var newPoint = Object.Instantiate(inputGameObject, inputVector3, Quaternion.identity, pointContainer.transform);
        _pointGameObjects.Add(pointID, newPoint);
    }

    /// //////////////////////////////////
    /// line
    /// //////////////////////////////////
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
            if (endPoint == null)
            {
                continue;
            }

            var start = new Vector3(startPoint.positionX, startPoint.positionY, 0);
            var end = new Vector3(endPoint.positionX, endPoint.positionY, 0);
            CreateLineObj(start, end, lineContainer);
        }
    }

    /// <summary>
    /// 创建连线的obj
    /// </summary>
    private void CreateLineObj(Vector3 start, Vector3 end, GameObject lineContainer)
    {
        var line = Object.Instantiate(AssetManager.Instance.LoadAsset<GameObject>("line.prefab"),
            lineContainer.transform);

        var lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;

        _lineList.Add(line);
    }

    /// //////////////////////////////////
    /// unit
    /// //////////////////////////////////
    /// <summary>
    /// 创建地图单位
    /// </summary>
    private void CreateUnit(string pointID)
    {
        var unitID = Model.PointModels[pointID].unitID;

        if (Model.UnitData[unitID].ifKindness)
        {
            StaticCreateTeam(pointID);
            return;
        }

        CreateEnemy(pointID);
    }

    /// <summary>
    /// 创建敌人
    /// </summary>
    private void CreateEnemy(string pointID)
    {
        //model
        var unitID = Model.PointModels[pointID].unitID;
        var unit = Model.UnitData[unitID];

        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        Model.EnemyModels.Add(pointID, unit);

        //view
        var point = Model.PointData[pointID];
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = 0
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath;
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer);
    }

    /// <summary>
    /// 初始化时通过静态数据创建我方队伍
    /// </summary>
    private void StaticCreateTeam(string pointID)
    {
        //model
        var unitID = Model.PointModels[pointID].unitID;
        var unit = Model.UnitData[unitID];

        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        Model.TeamModels.Add(pointID, unit);

        //view
        var point = Model.PointData[pointID];
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = 0
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath;
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer);
    }

    /// <summary>
    /// 游戏中动态创建我方队伍
    /// </summary>
    private void DynamicCreateTeam(string pointID, Unit unit)
    {
        //model
        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        Model.TeamModels.Add(pointID, unit);
        Model.PointModels[pointID].unitID = unit.unitID;

        //view
        var point = Model.PointData[pointID];
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = 0
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath;
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer);
    }

    /// <summary>
    /// 创建游戏单位obj
    /// </summary>
    private void CreateUnitObj(string pointID, Vector3 position, string path, GameObject unitContainer)
    {
        var sprite = AssetManager.Instance.LoadAsset<Sprite>(path);
        if (!unitContainer)
        {
            Debug.Log("unitContainer == null");
            return;
        }

        var unit = Object.Instantiate(AssetManager.Instance.LoadAsset<GameObject>("Unit.prefab"), position,
            Quaternion.identity, unitContainer.transform);
        unit.GetComponent<SpriteRenderer>().sprite = sprite;
        _unitGameObjects.Add(pointID, unit);
    }

    /// ////////////////////////////////////
    /// 事件
    /// ////////////////////////////////////
    /// <summary>
    /// 鼠标左键点击，检测是否有东西
    /// </summary>
    /// <param name="pos"></param>
    private void ClickLeft(Vector3 pos)
    {
        if (Camera.main is null)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(pos);
        var hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (!hit.collider)
        {
            return;
        }

        CNTPointBtnOnClickCheck(hit.collider.gameObject);
    }

    /// <summary>
    /// 检查PointGameObjects是否持有该物体
    /// </summary>
    /// <param name="inputGameObject"></param>
    private void CNTPointBtnOnClickCheck(GameObject inputGameObject)
    {
        if (!_pointGameObjects.ContainsValue(inputGameObject))
        {
            return;
        }

        foreach (var pair in _pointGameObjects)
        {
            if (Model.PointModels[pair.Key].unitID != "")
            {
                break;
            }

            if (!pair.Value.Equals(inputGameObject))
            {
                continue;
            }

            CNTPointBtnOnClick(pair.Key);
            break;
        }
    }

    /// <summary>
    /// CNTPoint左键点击事件
    /// </summary>
    private void CNTPointBtnOnClick(string pointID)
    {
        var unit = new Unit
        {
            unitID = "unit_01",
            PointID = new List<string>(),
            MemberID = new List<string>
            {
                "member_01",
                "member_02"
            },
            teamAp = 3,
            ifKindness = true
        };

        DynamicCreateTeam(pointID, unit);
        Model.StartAble = true;
        Model.PointData[pointID].canNewTeam = false;
        var point = AssetManager.Instance.LoadAsset<GameObject>("Point.prefab");
        ChangePoint(pointID, point);
        _ctrl.CheckStartBtnState();
        //todo: 新页面，编队与携带物资
    }

    /// ////////////////////////////////
    /// 工具
    /// ////////////////////////////////
    /// <summary>
    /// 通过ID获得点位model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private PointData GETPointDataByID(string id)
    {
        if (Model.PointModels.ContainsKey(id))
        {
            return Model.PointData[id];
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
        if (_pointGameObjects.ContainsKey(id))
        {
            return _pointGameObjects[id];
        }

        Debug.LogError("getPointByID is null");
        return null;
    }

    /// <summary>
    /// 改变点的类型
    /// </summary>
    private void ChangePoint(string pointID, GameObject inputGameObject)
    {
        var transform = _pointGameObjects[pointID].transform;
        Object.Destroy(_pointGameObjects[pointID]);
        _pointGameObjects.Remove(pointID);
        var newPoint = Object.Instantiate(inputGameObject, transform.position, transform.rotation, transform.parent);
        _pointGameObjects.Add(pointID, newPoint);
    }
}