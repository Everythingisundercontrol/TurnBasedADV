using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetManager : MonoSingleton<AssetManager>, IMonoManager
{
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
    }


    /// <summary>
    /// 通过string获取T类型的资源
    /// </summary>
    /// <param name="addressableKey"></param>
    /// <returns></returns>
    public T GetGameResource<T>(string addressableKey)
    {
        var res = Addressables.LoadAssetAsync<T>(addressableKey);
        return res.WaitForCompletion();
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public string ReadJsonFileToString(string jsonPath)
    {
        var path = Path.Combine(Application.persistentDataPath,jsonPath);
        var fileBytes = File.ReadAllBytes(path);
        var jsonString = Encoding.UTF8.GetString(fileBytes);
        return jsonString;
    }
}