using UnityEngine;

public abstract class UICtrlBase : MonoBehaviour
{
    public abstract void OnInit(params object[] param);

    public abstract void OpenRoot(params object[] param);

    public abstract void CloseRoot();

    public abstract void OnClear();

    public abstract void BindEvent();
}