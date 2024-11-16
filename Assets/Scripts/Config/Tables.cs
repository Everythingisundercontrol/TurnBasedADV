
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


public partial class Tables
{
    public CfgCamera CfgCamera {get; }
    public CfgPrefab CfgPrefab {get; }
    public CfgSprite CfgSprite {get; }
    public CfgUI CfgUI {get; }
    public CfgBGM CfgBGM {get; }
    public CfgSFX CfgSFX {get; }
    public CfgScene CfgScene {get; }

    public Tables(System.Func<string, ByteBuf> loader)
    {
        CfgCamera = new CfgCamera(loader("cfgcamera"));
        CfgPrefab = new CfgPrefab(loader("cfgprefab"));
        CfgSprite = new CfgSprite(loader("cfgsprite"));
        CfgUI = new CfgUI(loader("cfgui"));
        CfgBGM = new CfgBGM(loader("cfgbgm"));
        CfgSFX = new CfgSFX(loader("cfgsfx"));
        CfgScene = new CfgScene(loader("cfgscene"));
        ResolveRef();
    }
    
    private void ResolveRef()
    {
        CfgCamera.ResolveRef(this);
        CfgPrefab.ResolveRef(this);
        CfgSprite.ResolveRef(this);
        CfgUI.ResolveRef(this);
        CfgBGM.ResolveRef(this);
        CfgSFX.ResolveRef(this);
        CfgScene.ResolveRef(this);
    }
}


