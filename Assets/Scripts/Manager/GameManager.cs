using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    private List<IMonoManager> _managerList = new List<IMonoManager>();

    protected override void Awake()
    {
        base.Awake();
        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<FsmManager>();

        _managerList.Add(AssetManager.Instance);
        _managerList.Add(UIManager.Instance);
        _managerList.Add(FsmManager.Instance);
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
    }
}