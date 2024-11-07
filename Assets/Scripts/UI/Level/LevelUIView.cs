using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LevelUIView : MonoBehaviour
{
    public GameObject TeamInfo;
    public Button returnBtn;
    public Button startBtn;
    public Text Tp;
    public Image leaderPic;

    /// <summary>
    /// 打开页面
    /// </summary>
    public void OpenWindow()
    {
        StartBtnDisable();
        ShowTeamInfo(false);
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

    /// <summary>
    /// 队伍信息是否展示，true展示，false隐藏
    /// </summary>
    /// <param name="showRoHide"></param>
    public void ShowTeamInfo(bool showRoHide)
    {
        if (showRoHide)
        {
            TeamInfo.SetActive(true);
            return;
        }

        TeamInfo.SetActive(false);
    }

    /// <summary>
    /// 设置Ap的text
    /// </summary>
    public void SetTp(int Tp)
    {
    }
}