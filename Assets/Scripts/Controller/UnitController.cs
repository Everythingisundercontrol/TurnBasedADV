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
    /// 
    /// </summary>
    public void TurnEndEnemyMoveCheck()
    {
        var unitInfo = GetComponent<UnitInfo>();
        if (unitInfo.ifKindness)
        {
            return;
        }
        Debug.Log("TurnEndEnemyMoveCheck" + unitInfo.UnitID + unitInfo.ifKindness);
        var res = WarManager.Instance.Model.GetPointsWithinTwoSteps(unitInfo.LocatedPointID, 2);
        if (res.Count>0)
        {
            var list = WarManager.Instance.Model.PointsShortestPathCalculation(unitInfo.LocatedPointID, res.First());
            if (list == null)
            {
                list = WarManager.Instance.Model.PointsShortestPathCalculation(res.First(), unitInfo.LocatedPointID);
                if (list == null)
                {
                    Debug.Log("list == null");
                    return;
                }
        
                list.Reverse();
            }
            
            WarManager.Instance.EnemyStepMove(list[0],list[1]);

        }
    }
}