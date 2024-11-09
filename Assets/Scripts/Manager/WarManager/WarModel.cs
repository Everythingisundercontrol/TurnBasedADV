using System.Collections.Generic;

public class WarModel
{
    public Dictionary<string, Point> PointModels; //动态,key:pointID
    public Dictionary<string, Unit> TeamModels;
    public Dictionary<string, Unit> EnemyModels;

    public Dictionary<string, List<Member>> MemberModels; //动态,key:UnitID

    public Dictionary<string, PointData> PointData; //静态
    public Dictionary<string, Unit> UnitData;
    public Dictionary<string, Event> EventData;
    public Dictionary<string, Member> MemberData;

    public bool StartAble; //是否可以开始游戏
    public GameStateEnum GameState; //当前游戏状态

    public int TeamPoints; //队伍行动值
    public int Rounds; //回合数

    public Unit FocosOn;

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
        TeamPoints += Rounds * (TeamModels.Count + 1); //可能要改？？
        MemberAPReset();
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
    /// 加载动态数据
    /// </summary>
    private void LoadPointModel(string path)
    {
        var points = AssetManager.LoadJsonFile<PointList>(path).Points;

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
        var pointDatas = AssetManager.LoadJsonFile<PointDataList>("pointData.json").PointDatas;

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
        var units = AssetManager.LoadJsonFile<UnitList>("unitData.json").Units;
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
        var members = AssetManager.LoadJsonFile<MemberList>("memberData.json").Members;
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
        var events = AssetManager.LoadJsonFile<EventList>("eventData.json").Events;
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