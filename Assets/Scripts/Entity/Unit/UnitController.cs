using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private float _clickTimer;

    private void OnMouseDown()
    {
        _clickTimer = Time.time;
    }

    private void OnMouseUp()
    {
        if (!(Time.time - _clickTimer < 0.1f))
        {
            return;
        }

        var unitID = GetComponent<UnitInfo>().UnitID;
        var pointID = GetComponent<UnitInfo>().LocatedPointID;

        if (unitID != WarManager.Instance.Model.FocosOnUnitID)
        {
            WarManager.Instance.DecisionChangeFocosUnit(unitID, pointID);
        }
    }

    /// <summary>
    /// 回合结束敌人移动
    /// </summary>
    public void TurnEndEnemyMoveCheck()
    {
        var unitInfo = GetComponent<UnitInfo>();
        if (unitInfo.ifKindness)
        {
            return;
        }
        
        //搜索周围两格有无team
        var res = WarManager.Instance.Model.GetPointsWithinNLayers(unitInfo.LocatedPointID, 2);
        if (res.Count <= 0)
        {
            return;
        }
        var list = WarManager.Instance.Model.PointsShortestPathCalculation(unitInfo.LocatedPointID, res.First());
        if (list == null)
        {
            Debug.Log("list == null");
            return;
        }

        WarManager.Instance.EnemyStepMove(list[0], list[1]);
    }

    /// <summary>
    /// 移动改变model数据
    /// </summary>
    /// <param name="NextPointID"></param>
    public void MoveTo(string NextPointID)
    {
        GetComponent<UnitInfo>().LocatedPointID = NextPointID;
    }
}