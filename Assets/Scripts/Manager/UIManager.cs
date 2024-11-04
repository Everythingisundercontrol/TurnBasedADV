using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : BaseSingleton<UIManager>, IMonoManager
{
    private Dictionary<string, UICtrlBase> _allView = new Dictionary<string, UICtrlBase>();

    public void Start() //记得加异步
    {
    }

    public void OnInit()
    {
        OpenWindow("HomeView.prefab");
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

    public LevelUICtrl GetLevelUICtrl()
    {
        return GetCtrl<LevelUICtrl>("LevelView.prefab");
    }
    
    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    public void OpenWindow(string windowName, params object[] param)
    {
        var ctrl = GetCtrl<UICtrlBase>(windowName, param);
        if (!ctrl)
        {
            return;
        }

        ctrl.OpenRoot(param);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="windowName">窗口名</param>
    /// <param name="param">参数</param>
    public void CloseWindow(string windowName, params object[] param)
    {
        var ctrl = GetCtrl<UICtrlBase>(windowName, param);
        if (!ctrl)
        {
            return;
        }

        ctrl.CloseRoot();
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
        var window = AssetManager.Instance.LoadAsset<GameObject>(windowName);
        if (!window)
        {
            Debug.LogError(windowName + " 页面不存在");
            return null;
        }

        var gm = GameObject.Find("Manager");
        var rootObj = Object.Instantiate(window, gm.transform, true); //用他自己的ctrl管他自己
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