using System;
using System.Collections.Generic;

[Serializable]
public class PointData
{
    public string pointID;
    public bool canNewTeam; //是否能创建队伍
    public List<string> prePoints;
    public List<string> nextPoints; //该点连接的下一点
    public float positionX; //x轴位置
    public float positionY; //y轴位置
}