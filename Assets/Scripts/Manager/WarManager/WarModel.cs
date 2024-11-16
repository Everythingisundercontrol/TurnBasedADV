using System.Collections.Generic;

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
        var queue = new Queue<(Point, List<string>)>();
        queue.Enqueue((PointModels[start], new List<string> {start}));

        var visited = new HashSet<string>();

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();
            var currentPointID = current.pointID;

            if (visited.Contains(currentPointID))
            {
                continue;
            }

            visited.Add(currentPointID);
            if (currentPointID == end)
            {
                return path;
            }

            foreach (var nextPoint in PointData[currentPointID].nextPoints)
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
    // /// 计算两点间最短距离
    // /// </summary>
    // public int PointsDistanceCalculation(string start, string end)
    // {
    //     var distance = PointsShortestPathCalculation(start, end).Count;
    //     return distance - 1;
    // }

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
            if (!PointModels.ContainsKey(pointData.pointID))
            {
                continue;
            }

            if (PointData.ContainsKey(pointData.pointID))
            {
                continue;
            }

            PointData.Add(pointData.pointID, pointData);
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
            foreach (var pointID in PointModels.Keys)
            {
                if (UnitData.ContainsKey(pointID))
                {
                    continue;
                }

                if (!unit.PointID.Contains(pointID))
                {
                    continue;
                }

                UnitData.Add(unit.unitID, unit);
                PointModels[pointID].unitID = unit.unitID;
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
    /// 从json文件中加载事件    //todo:拆
    /// </summary>
    private void LoadEvent()
    {
        var events = AssetManager.Instance.LoadJsonFile<EventList>("eventData.json").Events;
        if (events == null)
        {
            return;
        }

        foreach (var eEvent in events)
        {
            foreach (var pointID in PointModels.Keys)
            {
                if (EventData.ContainsKey(pointID)) //data已经有了就不再录入
                {
                    continue;
                }

                if (!eEvent.pointID.Contains(pointID)) //这个事件没有这个点位就跳过
                {
                    continue;
                }

                EventData.Add(eEvent.eventID, eEvent);
                PointModels[pointID].eventID = eEvent.eventID;
            }
        }
    }
}