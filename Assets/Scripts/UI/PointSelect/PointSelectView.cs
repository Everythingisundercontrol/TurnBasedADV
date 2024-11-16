using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointSelectView : MonoBehaviour
{
    public Dictionary<string, GameObject> MapPointDic;
    public Button Mask;
    /// <summary>
    /// 打开窗口
    /// </summary>
    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void CloseWindow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}