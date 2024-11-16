using Luban;
using UnityEngine;

public class ConfigManager : BaseSingleton<ConfigManager>, IMonoManager
{
    public static Tables Tables;


    public void OnInit()
    {
        Tables = new Tables(LoadByteBuf);
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
    /// 从file中读取字节流
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static ByteBuf LoadByteBuf(string file)
    {
        return new ByteBuf(AssetManager.Instance.LoadAsset<TextAsset>($"Assets/AddressableAssets/Config/{file}.bytes").bytes);
    }
}