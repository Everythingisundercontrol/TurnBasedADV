using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Manager.BattleManager;
using UnityEngine;
using Object = UnityEngine.Object;

public class WarManager : BaseSingleton<WarManager>, IMonoManager
{
    public WarModel Model;

    private List<GameObject> _lineList; //不确定有没有用
    private Dictionary<string, GameObject> _pointGameObjects; //key:pointID
    private Dictionary<string, GameObject> _unitGameObjects; //key:pointID

    private LevelUICtrl _ctrl; //获取UI信息

    private string _level;
    private string _jsPath;


    public void OnInit()
    {
        Model = new WarModel();
        Model.OnInit();

        _lineList = new List<GameObject>();
        _pointGameObjects = new Dictionary<string, GameObject>();
        _unitGameObjects = new Dictionary<string, GameObject>();
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
    /// 进入关卡，赋值
    /// </summary>
    public void OnEnter(string level, string jsPath)
    {
        _level = level;
        _jsPath = jsPath;

        UIManager.Instance.OpenWindow("LevelView.prefab");

        _ctrl = UIManager.Instance.GetCtrl<LevelUICtrl>("LevelView.prefab");


        FsmManager.Instance.WarFsmOnOpen();
        FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_SetUpState);
    }

    /// <summary>
    /// 退出关卡,数据清空
    /// </summary>
    public void OnQuit()
    {
        _level = null;
        _jsPath = null;

        _lineList.Clear();
        _pointGameObjects.Clear();
        _unitGameObjects.Clear();

        FsmManager.Instance.WarFsmOnClose();
        _ctrl.OnClear();
        _ctrl = null;
        UIManager.Instance.CloseWindow("LevelView.prefab");

        Model.OnExit();
        //监听数值变化事件结束
        EventManager.Instance.RemoveListener(EventName.RoundsChange, RoundsChangeEvent);
        EventManager.Instance.RemoveListener(EventName.TpChange, TeamPointChangeEvent);
    }

    /// <summary>
    /// SetUp，加载数据，物体创建，事件绑定
    /// </summary>
    public void SetUpOnEnter()
    {
        Debug.Log("SetUpOnEnter");
        LoadMap(_level);
        Model.OnEnter(_jsPath);
        InitGameObject();
        SetUpBindEvent();
    }

    /// <summary>
    /// SetUp退出,把所有cntP变为普通点，事件绑定解除,游戏开始
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
            ChangePoint(pointID, Model.PointData[pointID]);
        }

        Model.GameStart();
        _ctrl.SetUpOnExitUI();

        //监听TP/Ap/rounds数值变化
        EventManager.Instance.AddListener(EventName.RoundsChange, RoundsChangeEvent);
        EventManager.Instance.AddListener(EventName.TpChange, TeamPointChangeEvent);
    }

    /// <summary>
    /// TurnInit进入，数据计算并reset，全部完成后进入decision状态
    /// </summary>
    public void TurnInitOnEnter()
    {
        Model.TurnStart();
        _ctrl.TurnInitOnEnterUI();
        FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_DecisionState);
    }

    /// <summary>
    /// TurnInit退出
    /// </summary>
    public void TurnInitOnExit()
    {
    }

    /// <summary>
    /// Decision进入
    /// </summary>
    public void DecisionOnEnter()
    {
        Debug.Log("DecisionOnEnter");
        _ctrl.DecisionOnEnterUI(); //UI事件绑定
        DecisionBindEvent(); //场景事件绑定
    }

    /// <summary>
    /// Decision退出
    /// </summary>
    public void DecisionOnExit()
    {
        _ctrl.DecisionOnExitUI();
    }

    /// <summary>
    /// EndTurn进入   //敌人行动、结束检测
    /// </summary>
    public void EndTurnOnEnter()
    {
        Debug.Log("EndTurnOnEnter : ");
        TurnEndEnemyMove();
        if (GameEndCheck())
        {
            FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_EndGameState);
            return;
        }

        FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_TurnInitState);
    }

    /// <summary>
    /// EndTurn退出
    /// </summary>
    public void EndTurnOnExit()
    {
    }

    /// <summary>
    /// EndGame进入
    /// </summary>
    public void EndGameOnEnter()
    {
        GameManager.Instance.StartCoroutine(WarEndSceneChange());
        UIManager.Instance.OpenWindow("EndGameView.prefab");
        OnQuit();
    }

    /// <summary>
    /// EndGame退出
    /// </summary>
    public void EndGameOnExit()
    {
    }

    /// <summary>
    /// Pause进入
    /// </summary>
    public void PauseOnEnter()
    {
    }

    /// <summary>
    /// Pause退出
    /// </summary>
    public void PauseOnExit()
    {
    }

    /// <summary>
    /// Battle进入
    /// </summary>
    public void BattleOnEnter()
    {
        //todo:
    }

    /// <summary>
    /// Battle退出
    /// </summary>
    public void BattleOnExit()
    {
    }

    /// <summary>
    /// 关卡界面开始按钮点击事件
    /// </summary>
    public void LevelStartBtnOnClickEvent()
    {
        FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_TurnInitState);
    }

    /// <summary>
    /// 关卡界面回合结束按钮点击事件
    /// </summary>
    public void LevelTurnEndBtnOnClickEvent()
    {
        FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_EndTurnState);
    }

    /// <summary>
    /// 队伍移动
    /// </summary>
    public void TeamMove(string destinationPointID)
    {
        var start = Model.FocosOnPointID;
        Debug.Log(start + "  :  " + destinationPointID);
        var list = Model.PointsShortestPathCalculation(start, destinationPointID);
        if (list == null)
        {
            Debug.Log("list == null");
            return;
        }

        var dis = list.Count - 1;
        if (dis <= 0)
        {
            Debug.Log("dis <= 0");
            return;
        }

        if (Model.TeamPoints < dis)
        {
            Debug.Log("TP不足" + Model.TeamPoints + " : " + dis);
            return;
        }

        // model
        Model.TeamPoints -= dis;

        for (var i = 0; i < list.Count - 1; i++)
        {
            TeamStepMove(list[i], list[i + 1]);
        }
    }

    /// <summary>
    /// 队伍移动结束
    /// </summary>
    public void MoveEventEnd()
    {
        _ctrl.MoveEventEnd();
    }

    /// <summary>
    /// 检查该点位是否可创建队伍并判断unit是否为空
    /// </summary>
    public void TeamCreateAbleCheck(string pointID)
    {
        if (!Model.PointData[pointID].canNewTeam)
        {
            return;
        }

        if (Model.PointModels[pointID].unitID != "")
        {
            return;
        }

        CNTPointBtnOnClick(pointID);
    }

    /// <summary>
    /// setUP阶段事件绑定
    /// </summary>
    private void SetUpBindEvent()
    {
        // EventManager.Instance.AddListener<Vector3>(EventName.ClickLeft, SetUpClickLeft);
    }

    /// <summary>
    /// decision阶段事件绑定
    /// </summary>
    private void DecisionBindEvent()
    {
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 游戏结束加载新场景
    /// </summary>
    /// <returns></returns>
    private IEnumerator WarEndSceneChange()
    {
        yield return SceneManager.Instance.ChangeSceneAsync("Home");
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

            StaticCreateUnit(pointModel.Key);
        }
    }

    /// <summary>
    /// 事件初始化   //暂时不做
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
        CreatePointsObj(pointPosition, pointID, point, null);
    }

    /// <summary>
    /// 创建可建立队伍点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateCntPoint(Vector3 pointPosition, string pointID)
    {
        var point = AssetManager.Instance.LoadAsset<GameObject>("CNTPoint.prefab");
        CreatePointsObj(pointPosition, pointID, point, "CNTPoint");
    }

    /// <summary>
    /// 创建事件点位
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="pointID"></param>
    private void CreateEventPoint(Vector3 pointPosition, string pointID)
    {
        var point = AssetManager.Instance.LoadAsset<GameObject>("EventPoint.prefab");
        CreatePointsObj(pointPosition, pointID, point, "EventPoint");
    }

    /// <summary>
    /// 创建point的obj
    /// </summary>
    private void CreatePointsObj(Vector3 inputVector3, string pointID, GameObject inputGameObject,
        [CanBeNull] string PointType)
    {
        var pointContainer = GameObject.Find("Points");
        if (!pointContainer)
        {
            return;
        }

        var newPoint = Object.Instantiate(inputGameObject, inputVector3, Quaternion.identity, pointContainer.transform);

        var newPointInfo = newPoint.GetComponent<PointInfo>();
        newPointInfo.pointID = pointID;

        newPoint.AddComponent<PointController>();
        newPoint.GetComponent<PointController>().PointType = PointType;

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
    /// 通过静态数据创建地图单位
    /// </summary>
    private void StaticCreateUnit(string pointID)
    {
        var unitID = Model.PointModels[pointID].unitID;

        if (Model.UnitData[unitID].ifKindness)
        {
            StaticCreateTeam(pointID);
            return;
        }

        StaticCreateEnemy(pointID);
    }

    /// <summary>
    /// 初始化时通过静态数据创建敌人
    /// </summary>
    private void StaticCreateEnemy(string pointID)
    {
        //model
        var unitID = Model.PointModels[pointID].unitID;
        var unit = Model.UnitData[unitID];

        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js);

        Model.EnemyModels.Add(unit.unitID, unit);

        FillInMemberModel(unit);

        //view
        var point = Model.PointData[pointID];
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = -1
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath;
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer, unit.unitID);
    }

    /// <summary>
    /// 初始化时通过静态数据创建我方队伍
    /// </summary>
    private void StaticCreateTeam(string pointID)
    {
        //model
        var unitID = Model.PointModels[pointID].unitID; //获取该点位上的unitID
        var unit = Model.UnitData[unitID]; //在UnitData中查找该unit

        var js = JsonUtility.ToJson(unit); //深拷贝
        unit = JsonUtility.FromJson<Unit>(js);

        Model.TeamModels.Add(unit.unitID, unit); //将该unit加入我方队伍动态数据中

        FillInMemberModel(unit); //填入member的数据

        //view
        var point = Model.PointData[pointID]; //获取点位data信息
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = -1
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath; //获取大头照的路径
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer, unit.unitID);
    }

    /// <summary>
    /// 游戏中动态创建我方队伍
    /// </summary>
    private void DynamicCreateTeam(string pointID, Unit unit)
    {
        //model
        var js = JsonUtility.ToJson(unit);
        unit = JsonUtility.FromJson<Unit>(js); //深拷贝

        Model.TeamModels.Add(unit.unitID, unit);
        Model.PointModels[pointID].unitID = unit.unitID;

        FillInMemberModel(unit);

        //view
        var point = Model.PointData[pointID];
        var position = new Vector3
        {
            x = point.positionX,
            y = point.positionY,
            z = -1
        };

        if (unit.MemberID.Count <= 0)
        {
            Debug.LogError("unit.Member is null");
            return;
        }

        var path = Model.MemberData[unit.MemberID[0]].prefabPath;
        var unitContainer = GameObject.Find("Units");

        CreateUnitObj(pointID, position, path, unitContainer, unit.unitID);

        Model.FocosOnPointID = pointID;
        Model.FocosOnUnitID = unit.unitID; //聚焦改变
    }

    /// <summary>
    /// 创建游戏单位obj
    /// </summary>
    private void CreateUnitObj(string pointID, Vector3 position, string path, GameObject unitContainer, string unitID)
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
        unit.GetComponent<UnitInfo>().UnitID = unitID;
        unit.GetComponent<UnitInfo>().LocatedPointID = pointID;
        unit.GetComponent<UnitInfo>().ifKindness = Model.UnitData[unitID].ifKindness;
        unit.AddComponent<UnitController>();

        _unitGameObjects.Add(pointID, unit);
    }

    /// <summary>
    /// 填入member动态数据
    /// </summary>
    private void FillInMemberModel(Unit unit)
    {
        var memberList = new List<Member>();

        foreach (var memberID in unit.MemberID)
        {
            var js = JsonUtility.ToJson(Model.MemberData[memberID]);
            var member = JsonUtility.FromJson<Member>(js);
            memberList.Add(member);
        }

        Model.MemberModels.Add(unit.unitID, memberList);
    }

    /// ////////////////////////////////////
    /// 事件
    /// ////////////////////////////////////
    /// <summary>
    /// CNTPoint左键点击事件
    /// </summary>
    private void CNTPointBtnOnClick(string pointID)
    {
        var unit = new Unit
        {
            unitID = "unit_01",
            MemberID = new List<string>
            {
                "member_01",
                "member_02"
            },
            teamAp = 3,
            ifKindness = true
        };


        Model.UnitData.Add("unit_01", unit);
        DynamicCreateTeam(pointID, unit);

        Model.StartAble = true;

        Model.PointData[pointID].canNewTeam = false;
        ChangePoint(pointID, Model.PointData[pointID]);

        _ctrl.CheckStartBtnState();
        //todo: 新页面，编队与携带物资
    }

    /// <summary>
    /// 决策阶段点击事件选择单位
    /// </summary>
    public void DecisionChangeFocosUnit(string unitID, string pointID)
    {
        if (!Model.TeamModels.ContainsKey(unitID))
        {
            return;
        }

        Model.FocosOnUnitID = unitID;
        Model.FocosOnPointID = pointID;
        _ctrl.ShowFocosOnUnit();
    }

    /// <summary>
    /// 我方队伍移动拆分成每一步
    /// </summary>
    public void TeamStepMove(string startPointID, string nextPointID)
    {
        CheckNextPointUnit(startPointID, nextPointID); //检测下一个点是否有单位，并作出相应动作

        if (!string.IsNullOrEmpty(Model.PointModels[nextPointID].eventID)) //检测下个点是否会触发事件
        {
            //todo:事件触发
            Debug.Log("事件触发");
        }
    }

    /// <summary>
    /// 敌人队伍每步移动
    /// </summary>
    /// <param name="startPointID"></param>
    /// <param name="nextPointID"></param>
    public void EnemyStepMove(string startPointID, string nextPointID)
    {
        
        if (!string.IsNullOrEmpty(Model.PointModels[nextPointID].unitID))
        {
            var nextPointUnitID = Model.PointModels[nextPointID].unitID;

            if (Model.EnemyModels.ContainsKey(nextPointUnitID))
            {
                //位置交换
                return;
            }

            if (!Model.TeamModels.ContainsKey(nextPointUnitID))
            {
                return;
            }
            Debug.Log("NO MOVE");
            FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_BattleState);
            Model.BattleModels.Add(new BattleModel
            {
                BattleEnemyID = Model.PointModels[startPointID].unitID,
                BattleTeamID = Model.PointModels[nextPointID].unitID
            });

            // Debug.Log("battle : " + Model.PointModels[nextPointID].unitID);
            // RemoveUnitByPointID(nextPointID);
            // EnemyMove(startPointID, nextPointID);
            
            return;
        }

        EnemyMove(startPointID, nextPointID);
    }

    /// <summary>
    /// 检测下一步是否有敌方单位/我方单位 
    /// </summary>
    /// <param name="startPointID"></param>
    /// <param name="nextPointID"></param>
    private void CheckNextPointUnit(string startPointID, string nextPointID)
    {
        if (!string.IsNullOrEmpty(Model.PointModels[nextPointID].unitID))
        {
            var nextPointUnitID = Model.PointModels[nextPointID].unitID;

            if (Model.TeamModels.ContainsKey(nextPointUnitID))
            {
                //位置交换
                return;
            }

            if (!Model.EnemyModels.ContainsKey(nextPointUnitID))
            {
                return;
            }

            FsmManager.Instance.SetFsmState(FsmEnum.warFsm, FsmStateEnum.War_BattleState);
            Model.BattleModels.Add(new BattleModel
            {
                BattleTeamID = Model.PointModels[startPointID].unitID,
                BattleEnemyID = Model.PointModels[nextPointID].unitID
            });


            // Debug.Log("battle : " + Model.PointModels[nextPointID].unitID);
            // RemoveUnitByPointID(nextPointID);
            // Move(startPointID, nextPointID);

            return;
        }

        Move(startPointID, nextPointID);
    }

    /// <summary>
    /// 进入战斗场景与页面
    /// </summary>
    /// <returns></returns>
    // private IEnumerator BattleStart()
    // {
    // }

    /// <summary>
    /// 单位移动动作
    /// </summary>
    /// <param name="startPointID"></param>
    /// <param name="nextPointID"></param>
    private void Move(string startPointID, string nextPointID)
    {
        Debug.Log(startPointID + " => " + nextPointID);

        //model
        Model.PointModels[nextPointID].unitID = Model.PointModels[startPointID].unitID;
        Model.PointModels[startPointID].unitID = null;
        Model.FocosOnPointID = nextPointID;
        _unitGameObjects[startPointID].GetComponent<UnitController>().MoveTo(nextPointID);

        //view
        _unitGameObjects[startPointID].transform.position = _pointGameObjects[nextPointID].transform.position;
        _unitGameObjects.Add(nextPointID, _unitGameObjects[startPointID]);
        _unitGameObjects.Remove(startPointID);
    }

    private void EnemyMove(string startPointID, string nextPointID)
    {
        Debug.Log(startPointID + " => " + nextPointID);

        //model
        Model.PointModels[nextPointID].unitID = Model.PointModels[startPointID].unitID;
        Model.PointModels[startPointID].unitID = null;
        _unitGameObjects[startPointID].GetComponent<UnitController>().MoveTo(nextPointID);

        //view
        _unitGameObjects[startPointID].transform.position = _pointGameObjects[nextPointID].transform.position;
        _unitGameObjects.Add(nextPointID, _unitGameObjects[startPointID]);
        _unitGameObjects.Remove(startPointID);
    }

    /// <summary>
    /// 回合数变化事件
    /// </summary>
    private void RoundsChangeEvent()
    {
        Debug.Log("RoundsChangeEvent");
    }

    /// <summary>
    /// TP变化事件
    /// </summary>
    private void TeamPointChangeEvent()
    {
        _ctrl.ShowTeamPoint();
    }

    /// <summary>
    /// 回合结束敌人移动
    /// </summary>
    private void TurnEndEnemyMove()
    {
        var enemy = new List<GameObject>();
        foreach (var unitPair in _unitGameObjects)
        {
            if (!unitPair.Value.GetComponent<UnitInfo>().ifKindness)
            {
                enemy.Add(unitPair.Value);
            }
        }

        foreach (var t in enemy)
        {
            t.GetComponent<UnitController>().TurnEndEnemyMoveCheck();
        }
    }

    /// <summary>
    /// 游戏结束检测
    /// </summary>
    /// <returns></returns>
    private bool GameEndCheck()
    {
        return Model.TeamModels.Count == 0 || Model.EnemyModels.Count == 0;
    }

    /// ////////////////////////////////
    /// 工具
    /// ////////////////////////////////
    /// <summary>
    /// 通过ID获得点位data
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
    /// 改变点的类型
    /// </summary>
    private void ChangePoint(string pointID, PointData pointData)
    {
        Object.Destroy(_pointGameObjects[pointID]);
        _pointGameObjects.Remove(pointID);
        CreatePoints(pointData);
    }

    /// <summary>
    /// 移除场景中Unit
    /// </summary>
    /// <param name="pointID"></param>
    private void RemoveUnitByPointID(string pointID)
    {
        var unitID = Model.PointModels[pointID].unitID;
        Model.PointModels[pointID].unitID = null;
        if (Model.TeamModels.ContainsKey(unitID))
        {
            Model.TeamModels.Remove(unitID);
        }

        if (Model.EnemyModels.ContainsKey(unitID))
        {
            Model.EnemyModels.Remove(unitID);
        }

        Object.Destroy(_unitGameObjects[pointID]);
        _unitGameObjects.Remove(pointID);
    }

    // /// <summary>
    // /// 通过UnitID获取PointID
    // /// </summary>
    // /// <returns></returns>
    // private string GetPointIDByUnitID(string UnitID)
    // {
    //     foreach (var pair in Model.PointModels)
    //     {
    //         if (pair.Value.unitID == UnitID)
    //         {
    //             return pair.Key;
    //         }
    //     }
    //
    //     return null;
    // }

    // /// <summary>
    // /// 获取的pointObj的PointID
    // /// </summary>
    // private string GetPointIdbyPoint(GameObject inputGameObject)
    // {
    //     foreach (var pair in _pointGameObjects)
    //     {
    //         if (pair.Value.Equals(inputGameObject))
    //         {
    //             return pair.Key;
    //         }
    //     }
    //
    //     return null;
    // }
    //
    // /// <summary>
    // /// 获取的UnitObj的pointID
    // /// </summary>
    // private string GetPointIDByUnit(GameObject inputGameObject)
    // {
    //     foreach (var pair in _unitGameObjects)
    //     {
    //         if (pair.Value.Equals(inputGameObject))
    //         {
    //             return pair.Key;
    //         }
    //     }
    //
    //     return null;
    // }
    // /// <summary>
    // /// 通过ID获得点位gameobject
    // /// </summary>
    // /// <param name="id"></param>
    // /// <returns></returns>
    // private GameObject GETPointViewByID(string id)
    // {
    //     if (_pointGameObjects.ContainsKey(id))
    //     {
    //         return _pointGameObjects[id];
    //     }
    //
    //     Debug.LogError("getPointByID is null");
    //     return null;
    // }
    // /// <summary>
    // /// 决策阶段鼠标左键点击
    // /// </summary>
    // private void DecisionClickLeft(Vector3 pos)
    // {
    //     if (Camera.main is null)
    //     {
    //         return;
    //     }
    //
    //     var ray = Camera.main.ScreenPointToRay(pos);
    //     var hit = Physics2D.Raycast(ray.origin, ray.direction);
    //     if (!hit.collider)
    //     {
    //         return;
    //     }
    // }
    //
    //
    // /// <summary>
    // /// 获取点击的Point的ID
    // /// </summary>
    // /// <param name="pos"></param>
    // /// <returns></returns>
    // private string GetClickLeftPointID(Vector3 pos)
    // {
    //     if (Camera.main is null)
    //     {
    //         Debug.Log("Camera.main is null");
    //         return null;
    //     }
    //
    //     var ray = Camera.main.ScreenPointToRay(pos);
    //     var hit = Physics2D.Raycast(ray.origin, ray.direction);
    //     if (!hit.collider)
    //     {
    //         Debug.Log("!hit.collider");
    //         return null;
    //     }
    //
    //     var pointID = PointCheck(hit.collider.gameObject);
    //     if (pointID != null)
    //     {
    //         return pointID;
    //     }
    //
    //     pointID = UnitCheck(hit.collider.gameObject);
    //
    //     return pointID;
    // }

    // /// <summary>
    // /// 检测是否是点位
    // /// </summary>
    // private string PointCheck(GameObject inputGameObject)
    // {
    //     if (!_pointGameObjects.ContainsValue(inputGameObject))
    //     {
    //         return null;
    //     }
    //
    //     var pointID = GetPointIdbyPoint(inputGameObject);
    //     return pointID;
    // }

    // /// <summary>
    // /// 检测是否是单位
    // /// </summary>
    // private string UnitCheck(GameObject inputGameObject)
    // {
    //     if (!_unitGameObjects.ContainsValue(inputGameObject))
    //     {
    //         return null;
    //     }
    //
    //     var PointID = GetPointIDByUnit(inputGameObject);
    //     return PointID;
    // }
}