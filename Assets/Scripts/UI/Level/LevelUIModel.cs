

using System.Collections.Generic;

public class LevelUIModel
{
    public Dictionary<string, Point> PointModels;       //动态

    public Dictionary<string, PointData> PointData;
    public Dictionary<string, Unit> UnitData;
    public Dictionary<string, Event> EventData;

    
    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        PointModels = new Dictionary<string, Point>();
        PointData = new Dictionary<string, PointData>();
        UnitData = new Dictionary<string, Unit>();
        EventData = new Dictionary<string, Event>();
    }

    /// <summary>
    /// 打开时
    /// </summary>
    public void OnOpen()
    {
    }

    /// <summary>
    /// 关闭当前关卡
    /// </summary>
    public void OnClose()
    {
        PointModels = new Dictionary<string, Point>();
    }
    
}
