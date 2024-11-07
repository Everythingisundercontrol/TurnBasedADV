using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    private List<IMonoManager> _managerList = new List<IMonoManager>();
    // private UIManager _uiManager;

    protected override void Awake()
    {
        base.Awake();

        _managerList.Add(AssetManager.Instance);
        _managerList.Add(UIManager.Instance);
        _managerList.Add(FsmManager.Instance);
        _managerList.Add(InputManager.Instance);
        _managerList.Add(EventManager.Instance);
        _managerList.Add(SceneManager.Instance);
        _managerList.Add(WarManager.Instance);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
    }

    protected void Start()
    {
        foreach (var manager in _managerList)
        {
            manager.OnInit();
        }

        StartCoroutine(StartEnter());
    }

    protected void Update()
    {
        foreach (var manager in _managerList)
        {
            manager.Update();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 加载新场景并且进入Start
    /// </summary>
    /// <returns></returns>
    private static IEnumerator StartEnter()
    {
        yield return SceneManager.Instance.ChangeSceneAsync("Home");
        UIManager.Instance.OpenWindow("HomeView.prefab");
    }
}