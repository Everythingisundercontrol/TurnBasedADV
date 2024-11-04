using UnityEngine;
using Button = UnityEngine.UI.Button;

public class LevelUIView : MonoBehaviour
{
    public Button returnBtn;
    public Button startBtn;

    /// <summary>
    /// 打开页面
    /// </summary>
    public void OpenWindow()
    {
        StartBtnDisable();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭页面
    /// </summary>
    public void CloseWindow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        gameObject.SetActive(false);
    }
    
    public void StartBtnEnable()
    {
        startBtn.interactable = true;
    }

    public void StartBtnDisable()
    {
        startBtn.interactable = false;
    }
    
}