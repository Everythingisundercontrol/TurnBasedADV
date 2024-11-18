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
        Debug.Log("UnitController");
        if (!(Time.time - _clickTimer < 0.1f))
        {
            return;
        }

        var unitID = GetComponent<UnitInfo>().UnitID;
        var pointID = GetComponent<UnitInfo>().LocatedPointID;
        Debug.Log(unitID + " + " + pointID);
        if (unitID != WarManager.Instance.Model.FocosOnUnitID)
        {
            WarManager.Instance.DecisionChangeFocosUnit(unitID, pointID);
        }
    }
}