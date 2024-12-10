using UnityEngine;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour
{
    public GameObject GameOutComeObj;
    public GameObject ReturnBtnObj;
    
    /// <summary>
    /// 打开页面
    /// </summary>
    public void OpenWindow()
    {
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

    public void ShowGameOutCome(string outCome)
    {
        var text = GameOutComeObj.GetComponent<Text>();
        text.text = outCome;
    }
}