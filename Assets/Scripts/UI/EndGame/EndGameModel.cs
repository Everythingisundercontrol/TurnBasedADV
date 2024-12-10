public class EndGameModel
{
    public string GameOutComeText;

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
    }

    /// <summary>
    /// 打开时
    /// </summary>
    public void OnOpen()
    {
        SetGameOutComeText();
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetGameOutComeText()
    {
        if (WarManager.Instance.Model.TeamModels.Count == 0)
        {
            GameOutComeText = "失败";
            return;
        }

        if (WarManager.Instance.Model.EnemyModels.Count == 0)
        {
            GameOutComeText = "胜利";
        }
    }
}