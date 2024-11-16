
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public sealed partial class RowCfgSFX : Luban.BeanBase
{
    public RowCfgSFX(ByteBuf _buf) 
    {
        Id = _buf.ReadString();
        SFXType = (DefSFXType)_buf.ReadInt();
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);ClipPaths = new System.Collections.Generic.List<string>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { string _e0;  _e0 = _buf.ReadString(); ClipPaths.Add(_e0);}}
        Volume = _buf.ReadFloat();
        OneShot = _buf.ReadBool();
        Loop = _buf.ReadBool();
    }

    public static RowCfgSFX DeserializeRowCfgSFX(ByteBuf _buf)
    {
        return new RowCfgSFX(_buf);
    }

    /// <summary>
    /// sfx名
    /// </summary>
    public readonly string Id;
    /// <summary>
    /// 音效类别
    /// </summary>
    public readonly DefSFXType SFXType;
    /// <summary>
    /// 资源路径
    /// </summary>
    public readonly System.Collections.Generic.List<string> ClipPaths;
    /// <summary>
    /// 初始音量
    /// </summary>
    public readonly float Volume;
    /// <summary>
    /// 是否oneShot
    /// </summary>
    public readonly bool OneShot;
    /// <summary>
    /// 是否循环
    /// </summary>
    public readonly bool Loop;
   
    public const int __ID__ = -1749700645;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "SFXType:" + SFXType + ","
        + "clipPaths:" + Luban.StringUtil.CollectionToString(ClipPaths) + ","
        + "volume:" + Volume + ","
        + "oneShot:" + OneShot + ","
        + "loop:" + Loop + ","
        + "}";
    }
}


