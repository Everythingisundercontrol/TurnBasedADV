using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LevelUIView : MonoBehaviour
{
    public GameObject map;
    public List<GameObject> lineList;

    public Button returnBtn;
    public Button startBtn;

    public Dictionary<string, GameObject> PointGameObjects;

    public Dictionary<string, GameObject> GameObjectPrefabs;

    public Dictionary<string, GameObject> UnitGameObjects;


    /// <summary>
    /// 打开页面
    /// </summary>
    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭页面
    /// </summary>
    public void CloseWindow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 关闭当前关卡
    /// </summary>
    public void OnClose()
    {
        Destroy(map);
        foreach (var line in lineList)
        {
            Destroy(line);
        }

        foreach (var pair in PointGameObjects)
        {
            Destroy(pair.Value);
        }

        PointGameObjects = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// 初始化Dictionary
    /// </summary>
    public void InitDic()
    {
        lineList = new List<GameObject>();

        PointGameObjects = new Dictionary<string, GameObject>();

        if (GameObjectPrefabs != null)
        {
            return;
        }

        GameObjectPrefabs = new Dictionary<string, GameObject>();
        GameObjectPrefabs.Add("Point", AssetManager.Instance.GetGameResource<GameObject>("Point.prefab"));
        GameObjectPrefabs.Add("CNTPoint", AssetManager.Instance.GetGameResource<GameObject>("CNTPoint.prefab"));
        GameObjectPrefabs.Add("EventPoint", AssetManager.Instance.GetGameResource<GameObject>("EventPoint.prefab"));
        GameObjectPrefabs.Add("Line", AssetManager.Instance.GetGameResource<GameObject>("line.prefab"));
        GameObjectPrefabs.Add("Unit", AssetManager.Instance.GetGameResource<GameObject>("Unit.prefab"));
    }

    /// <summary>
    /// 初始化加载地图
    /// </summary>
    /// <param name="mapPrefab"></param>
    public void InitMap(GameObject mapPrefab)
    {
        var mapContainer = GameObject.Find("Map");
        if (!mapContainer)
        {
            return;
        }

        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, mapContainer.transform);
    }


    /// <summary>
    /// 创建point的obj
    /// </summary>
    public void CreatePointsObj(Vector3 inputVector3, string pointID, GameObject inputGameObject)
    {
        var pointContainer = GameObject.Find("Points");
        if (!pointContainer)
        {
            return;
        }

        var newPoint = Instantiate(inputGameObject, inputVector3, Quaternion.identity, pointContainer.transform);
        PointGameObjects.Add(pointID, newPoint);
    }

    /// <summary>
    /// 创建连线的obj
    /// </summary>
    public void CreateLineObj(Vector3 start, Vector3 end, GameObject lineContainer)
    {
        var line = Instantiate(GameObjectPrefabs["Line"], lineContainer.transform);

        var lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;

        lineList.Add(line);
    }

    /// <summary>
    /// 创建游戏单位obj//一模一样怎么办？
    /// </summary>
    public void CreateUnitObj(string pointID, Vector3 position, string prefabPath, GameObject unitContainer)
    {
        var prefab = AssetManager.Instance.GetGameResource<GameObject>(prefabPath);
        var unit = Instantiate(prefab, position, Quaternion.identity, unitContainer.transform);

        UnitGameObjects.Add(pointID, unit);
    }
}