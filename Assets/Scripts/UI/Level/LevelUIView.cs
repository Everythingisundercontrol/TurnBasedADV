using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LevelUIView : MonoBehaviour
{
    public GameObject teamInfo;
    public Button returnBtn;
    public GameObject TurnEndBtnObj;
    public Button TurnEndBtn;
    public GameObject startBtnObj;
    public Button startBtn;
    public Button MoveBtn;
    public Text tp;
    public Image leaderPic;

    /// <summary>
    /// 打开页面
    /// </summary>
    public void OpenWindow()
    {
        StartBtnDisable();
        ShowTeamInfo(false);
        TurnEndBtnObj.SetActive(false);
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
            teamInfo.SetActive(true);
            return;
        }

        teamInfo.SetActive(false);
    }

    /// <summary>
    /// 设置Ap的text
    /// </summary>
    public void SetTp(int tp)
    {
        this.tp.text = "[TP: " + tp + " ]";
    }

    /// <summary>
    /// 设置LeaderPic
    /// </summary>
    public void SetLeaderPic(Sprite inputLeaderPic)
    {
        leaderPic.sprite = inputLeaderPic;
        leaderPic.color = Color.white;
        Debug.Log("SetLeaderPic" + inputLeaderPic.name);
    }
}