using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoSingleton<UIManager>, IMonoManager
{
    private Dictionary<string, GameObject> _windowPrefabDic = new Dictionary<string, GameObject>();
    private Dictionary<string, UICtrlBase> _allView = new Dictionary<string, UICtrlBase>();

    public void Start() //记得加异步
    {
        Debug.Log("UI.Start");
        InitView();
    }

    public void OnInit()
    {
        Debug.Log("UI.OnInit");
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void OnClear()
    {
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    public void OpenWindow(string windowName, params object[] param)
    {
        if (!_windowPrefabDic.ContainsKey(windowName)) //检查是否有这个UI页面
        {
            return;
        }

        var ctrl = GetCtrl<UICtrlBase>(windowName, param);
        ctrl.OpenRoot(param);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    public void CloseWindow(string windowName, params object[] param)
    {
        if (!_windowPrefabDic.ContainsKey(windowName)) //检查是否有这个UI页面
        {
            return;
        }

        var ctrl = GetCtrl<UICtrlBase>(windowName, param);
        ctrl.CloseRoot();
    }
    

    /// <summary>
    /// 加载界面
    /// </summary>
    private void InitView()
    {
        _windowPrefabDic.Add("Home", AssetManager.Instance.GetGameResource<GameObject>("HomeView.prefab"));
        _windowPrefabDic.Add("Area", AssetManager.Instance.GetGameResource<GameObject>("AreaView.prefab"));
        _windowPrefabDic.Add("Level", AssetManager.Instance.GetGameResource<GameObject>("LevelView.prefab"));
        if (_windowPrefabDic.ContainsKey("Home"))
        {
            OpenWindow("Home");
        }
    }

    /// <summary>
    /// 获得页面的UICtrl，没有就创建
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    /// <typeparam name="T">泛型T，必须是UICtrlBase的继承类</typeparam>
    /// <returns></returns>
    private T GetCtrl<T>(string windowName, params object[] param) where T : UICtrlBase
    {
        if (_allView.ContainsKey(windowName))
        {
            return (T) _allView[windowName];
        }

        return CreateUIPage<T>(windowName, param);
    }

    /// <summary>
    /// 创建页面
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    /// <typeparam name="T">泛型T，必须是UICtrlBase的继承类</typeparam>
    /// <returns></returns>
    private T CreateUIPage<T>(string windowName, params object[] param) where T : UICtrlBase
    {
        var rootObj = Instantiate(_windowPrefabDic[windowName]); //用他自己的ctrl管他自己

        rootObj.SetActive(false);
        var canvas = rootObj.GetComponent<Canvas>();
        if (canvas)
        {
            canvas.worldCamera = Camera.main;
        }

        //SortingOrder先写死在页面prefab里
        T ctrlNew = null;
        var components = rootObj.GetComponents<Component>();
        foreach (var comp in components)
        {
            Debug.Log(comp.name);
            if (!(comp is UICtrlBase))
            {
                continue;
            }

            ctrlNew = comp as T;
            break;
        }

        if (!ctrlNew)
        {
            Debug.LogError("找不到ViewObj挂载的ctrl：" + rootObj.name);
            return null;
        }

        _allView.Add(windowName, ctrlNew);
        ctrlNew.OnInit(param);
        ctrlNew.BindEvent();
        return ctrlNew;
    }
}