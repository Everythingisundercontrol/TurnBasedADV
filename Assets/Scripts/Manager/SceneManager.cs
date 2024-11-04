using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneManager : BaseSingleton<SceneManager>, IMonoManager
{
    private readonly Dictionary<string, SceneInstance> _loadedSceneDic = new Dictionary<string, SceneInstance>();

    public void OnInit()
    {
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
        _loadedSceneDic.Clear();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="scenePath">场景的资源路径</param>
    /// <returns></returns>
    public IEnumerator ChangeSceneAsync(string scenePath)
    {
        //single模式切换场景
        yield return AssetManager.Instance.LoadSceneSync(scenePath, (sceneInstance) =>
        {
            // DOTween.KillAll();
            if (!_loadedSceneDic.ContainsKey(scenePath))
            {
                _loadedSceneDic.Add(scenePath, sceneInstance);
            }

            // 激活加载的场景
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(sceneInstance.Scene);
            EventManager.Instance.Dispatch(EventName.ChangeScene);
        });
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    /// <param name="scenePath"> 场景名称 </param>
    private void UnloadScene(string scenePath)
    {
        AssetManager.Instance.UnloadScene(_loadedSceneDic[scenePath]);
        _loadedSceneDic.Remove(scenePath);
    }
}