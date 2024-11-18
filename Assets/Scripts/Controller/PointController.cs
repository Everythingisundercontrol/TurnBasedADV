using UnityEngine;

public class PointController : MonoBehaviour
{
    public string PointType;
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

        Debug.Log(PointType);
        if (PointType != "CNTPoint")
        {
            return;
        }

        var pointID = GetComponent<PointInfo>().pointID;
        WarManager.Instance.TeamCreateAbleCheck(pointID);
    }
}