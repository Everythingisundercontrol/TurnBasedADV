
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public partial class CfgUI
{
    private readonly System.Collections.Generic.Dictionary<string, RowCfgUI> _dataMap;
    private readonly System.Collections.Generic.List<RowCfgUI> _dataList;
    
    public CfgUI(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, RowCfgUI>();
        _dataList = new System.Collections.Generic.List<RowCfgUI>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            RowCfgUI _v;
            _v = RowCfgUI.DeserializeRowCfgUI(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, RowCfgUI> DataMap => _dataMap;
    public System.Collections.Generic.List<RowCfgUI> DataList => _dataList;

    public RowCfgUI GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public RowCfgUI Get(string key) => _dataMap[key];
    public RowCfgUI this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}



