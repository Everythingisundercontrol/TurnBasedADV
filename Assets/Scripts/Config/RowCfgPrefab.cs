
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public sealed partial class RowCfgPrefab : Luban.BeanBase
{
    public RowCfgPrefab(ByteBuf _buf) 
    {
        Id = _buf.ReadString();
        PrefabPath = _buf.ReadString();
    }

    public static RowCfgPrefab DeserializeRowCfgPrefab(ByteBuf _buf)
    {
        return new RowCfgPrefab(_buf);
    }

    /// <summary>
    /// prefab名
    /// </summary>
    public readonly string Id;
    /// <summary>
    /// 资源路径
    /// </summary>
    public readonly string PrefabPath;
   
    public const int __ID__ = -1653575058;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "prefabPath:" + PrefabPath + ","
        + "}";
    }
}


