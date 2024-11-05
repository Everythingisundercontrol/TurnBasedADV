using System.Collections.Generic;

public class WarModel
{
    public Dictionary<string, Point> PointModels; //动态
    public Dictionary<string, Unit> TeamModels;
    public Dictionary<string, Unit> EnemyModels;

    public Dictionary<string, PointData> PointData; //静态
    public Dictionary<string, Unit> UnitData;
    public Dictionary<string, Event> EventData;
    public Dictionary<string, Member> MemberData;

    public bool StartAble;
    public bool ifWin;

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        PointModels = new Dictionary<string, Point>();
        TeamModels = new Dictionary<string, Unit>(); //key是pointID
        EnemyModels = new Dictionary<string, Unit>(); //key是pointID

        PointData = new Dictionary<string, PointData>();
        UnitData = new Dictionary<string, Unit>(); //key是unitID
        EventData = new Dictionary<string, Event>();
        MemberData = new Dictionary<string, Member>();
    }
    
}