using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    private List<IMonoManager> _managerList = new List<IMonoManager>();
    private UIManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        _managerList.Add(AssetManager.Instance);
        _managerList.Add(UIManager.Instance);
        _managerList.Add(FsmManager.Instance);
        _managerList.Add(InputManager.Instance);
        _managerList.Add(EventManager.Instance);
        _managerList.Add(SceneManager.Instance);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
    }

    protected void Start()
    {
        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }
    }

    protected void Update()
    {
        foreach (var manager in _managerList)
        {
            manager.Update();
        }
    }

    /// <summary>
    /// 进入war
    /// </summary>
    /// <param name="level"></param>
    /// <param name="jsPath"></param>
    public void InitWar(string level, string jsPath)
    {
        WarManager.Instance.OnInit(level, jsPath);
        _managerList.Add(WarManager.Instance);
    }

    /// <summary>
    /// 退出war   //todo:是否要将warManager的Instance销毁？
    /// </summary>
    public void QuitWar()
    {
        WarManager.Instance.OnClear();
    }
}