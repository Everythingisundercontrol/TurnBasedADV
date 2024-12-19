using System.Collections.Generic;
using Manager.BattleManager;

public class WarModel
{
    public Dictionary<string, Point> PointModels; //动态数据，但从配置文件中获取数据,key:pointID

    public Dictionary<string, Unit> TeamModels; //动态,key:UnitID
    public Dictionary<string, Unit> EnemyModels; //动态,key:UnitID

    public Dictionary<string, List<Member>> MemberModels; //动态,key:UnitID

    public Dictionary<string, PointData> PointData; //静态,key:PointID
    public Dictionary<string, Unit> UnitData; //静态,key:UnitID
    public Dictionary<string, Event> EventData; //静态,key:EventID
    public Dictionary<string, Member> MemberData; //静态,key:MemberID

    public bool StartAble; //是否可以开始游戏
    public GameStateEnum GameState; //当前游戏状态

    public List<BattleModel> BattleModels;

    public int TeamPoints
    {
        get => _teamPoints;
        set
        {
            if (_teamPoints == value)
            {
                return;
            }

            _teamPoints = value;
            EventManager.Instance.Dispatch(EventName.TpChange);
        }
    }

    public int Rounds
    {
        get => _rounds;
        set
        {
            if (_rounds == value)
            {
                return;
            }

            _rounds = value;
            EventManager.Instance.Dispatch(EventName.RoundsChange);
        }
    }

    public string FocosOnPointID; //聚焦点位ID
    public string FocosOnUnitID; //聚焦单位ID


    private int _teamPoints; //队伍行动值
    private int _rounds; //回合数

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        PointModels = new Dictionary<string, Point>();
        TeamModels = new Dictionary<string, Unit>(); //key是pointID
        EnemyModels = new Dictionary<string, Unit>(); //key是pointID

        MemberModels = new Dictionary<string, List<Member>>();

        PointData = new Dictionary<string, PointData>();
        UnitData = new Dictionary<string, Unit>(); //key是unitID
        EventData = new Dictionary<string, Event>();
        MemberData = new Dictionary<string, Member>();

        BattleModels = new List<BattleModel>();

        GameState = GameStateEnum.Default;
    }

    /// <summary>
    /// 进入，数据加载
    /// </summary>
    public void OnEnter(string path)
    {
        LoadPointModel(path);
        LoadData();
    }

    /// <summary>
    /// 退出，清空所有数据
    /// </summary>
    public void OnExit()
    {
        PointModels.Clear();
        TeamModels.Clear();
        EnemyModels.Clear();
        MemberModels.Clear();
        PointData.Clear();
        UnitData.Clear();
        EventData.Clear();
        MemberData.Clear();
        BattleModels.Clear();

        GameState = GameStateEnum.Default;
        StartAble = false;
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void GameStart()
    {
        TeamPoints = 0;
        Rounds = 0;
        GameState = GameStateEnum.Gaming;
    }

    /// <summary>
    /// 回合开始,数据更新，todo:buff更新
    /// </summary>
    public void TurnStart()
    {
        Rounds++;
        TeamPoints = Rounds * TeamModels.Count; //可能要改？？
        MemberAPReset();
    }

    /// <summary>
    /// 广度优先BFS计算两点之间最短路径
    /// </summary>
    public List<string> PointsShortestPathCalculation(string start, string end)
    {
        var queue = new Queue<(Point, List<string>)>(); //队列储存待处理的点和到达该点的路径
        queue.Enqueue((PointModels[start], new List<string> {start})); //将初始点加入该队列

        var visited = new HashSet<string>(); //访问过的点

        while (queue.Count > 0) //当队列不空
        {
            var (current, path) = queue.Dequeue(); //第一个元素出队
            var currentPointID = current.pointID; //该元素PointID

            if (visited.Contains(currentPointID)) //若该点位已访问过，则跳过该点
            {
                continue;
            }

            visited.Add(currentPointID); //该点位变为已访问过
            if (currentPointID == end) //若该点位是目标点位，则退出循环
            {
                return path;
            }

            foreach (var nextPoint in PointData[currentPointID].nextPoints) //查找下一可到达的未访问过的点位，并加入队列
            {
                if (!visited.Contains(nextPoint))
                {
                    queue.Enqueue((PointModels[nextPoint], new List<string>(path) {nextPoint}));
                }
            }
        }

        return null;
    }

    // /// <summary>
    // /// 敌人最近n层范围检测我方队伍
    // /// </summary>
    // /// <returns></returns>
    // public HashSet<string> GetPointsWithinNLayerss(string startId, int layers)
    // {
    //     var visited = new HashSet<string>(); //访问过的点
    //     var result = new HashSet<string>(); //指定步数内可达的所有pointID
    //     var queue = new Queue<(PointData, int)>(); // 队列中存储元组，包含点和当前层级
    //     var startPoint = PointData[startId];
    //
    //     if (startPoint != null) //如果startPoint不为null，则将其加入队列，并标记为已访问
    //     {
    //         queue.Enqueue((startPoint, 0)); // 起始点的层级为0
    //         visited.Add(startPoint.pointID);
    //     }
    //
    //     // while (queue.Count > 0) // 只要队列不为空
    //     // {
    //     //     var (currentPoint, currentLayer) = queue.Dequeue();//队列队首元素出列
    //     //
    //     //     // 检查属性，若该点的unit不为空且是我方队伍，则result加入该点
    //     //     if (!string.IsNullOrEmpty(PointModels[currentPoint.pointID].unitID) &&
    //     //         TeamModels.ContainsKey(PointModels[currentPoint.pointID].unitID))
    //     //     {
    //     //         result.Add(currentPoint.pointID);
    //     //     }
    //     //
    //     //     //若下一层层级超出上限，则不列入队列
    //     //     if (currentLayer + 1 > layers)
    //     //     {
    //     //         continue;
    //     //     }
    //     //     
    //     //     foreach (var nextId in currentPoint.nextPoints)
    //     //     {
    //     //         if (!PointData.ContainsKey(nextId) || visited.Contains(nextId))
    //     //         {
    //     //             continue;
    //     //         }
    //     //
    //     //         queue.Enqueue((PointData[nextId], currentLayer + 1));
    //     //         visited.Add(nextId);
    //     //     }
    //     // }
    //
    //     return result;
    // }

    /// <summary>
    /// 计算两点间最短距离
    /// </summary>
    public int PointsDistanceCalculation(string start, string end)
    {
        var distance = PointsShortestPathCalculation(start, end).Count;
        return distance - 1;
    }

    /// <summary>
    /// 成员Ap重置
    /// </summary>
    private void MemberAPReset()
    {
        foreach (var Members in MemberModels.Values)
        {
            foreach (var member in Members)
            {
                member.Ap = MemberData[member.memberID].Ap;
            }
        }
    }

    /// <summary>
    /// 加载point的动态数据
    /// </summary>
    private void LoadPointModel(string path)
    {
        var points = AssetManager.Instance.LoadJsonFile<PointList>(path).Points;

        foreach (var point in points)
        {
            PointModels.Add(point.pointID, point);
        }
    }

    /// <summary>
    /// 静态数据读入加载
    /// </summary>
    private void LoadData()
    {
        LoadPointData();
        LoadUnit();
        LoadMember();
        LoadEvent();
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
            if (!PointModels.ContainsKey(pointData.pointID)) //若关卡文件没有该点ID，则不读取该点数据
            {
                continue;
            }

            if (PointData.ContainsKey(pointData.pointID)) //若该点已被加载过，则跳过
            {
                continue;
            }

            PointData.Add(pointData.pointID, pointData); //将该点数据加入pointdata中
        }
    }

    /// <summary>
    /// 从json文件中加载单位
    /// </summary>
    private void LoadUnit()
    {
        var units = AssetManager.Instance.LoadJsonFile<UnitList>("unitData.json").Units;
        if (units == null)
        {
            return;
        }

        foreach (var point in PointModels.Values)
        {
            if (string.IsNullOrEmpty(point.unitID))
            {
                continue;
            }

            foreach (var unit in units)
            {
                if (unit.unitID != point.unitID)
                {
                    continue;
                }

                UnitData.Add(unit.unitID, unit);
                break;
            }
        }
    }

    /// <summary>
    /// 从json文件中加载角色
    /// </summary>
    private void LoadMember()
    {
        var members = AssetManager.Instance.LoadJsonFile<MemberList>("memberData.json").Members;
        if (members == null)
        {
            return;
        }

        //todo:待优化，有筛选地填入
        foreach (var member in members)
        {
            MemberData.Add(member.memberID, member);
        }
    }

    /// <summary>
    /// 从json文件中加载事件
    /// </summary>
    private void LoadEvent()
    {
        var events = AssetManager.Instance.LoadJsonFile<EventList>("eventData.json").Events;
        if (events == null)
        {
            return;
        }

        foreach (var point in PointModels.Values)
        {
            if (string.IsNullOrEmpty(point.unitID))
            {
                continue;
            }

            foreach (var eEvent in events)
            {
                if (eEvent.eventID != point.eventID)
                {
                    continue;
                }

                EventData.Add(eEvent.eventID, eEvent);
                break;
            }
        }
    }
}