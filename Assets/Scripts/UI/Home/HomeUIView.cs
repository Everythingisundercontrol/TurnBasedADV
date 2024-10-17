using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUIView : MonoBehaviour
{
    public Button settingBtn;
    public Button startBtn;

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
        // GameManager.Instance.StartCoroutine(CloseWindowIEnumerator());
    }

    // /// <summary>
    // /// 等两秒再关闭
    // /// </summary>
    // /// <returns></returns>
    // private IEnumerator CloseWindowIEnumerator()
    // {
    //     yield return new WaitForSeconds(2);
    //     gameObject.SetActive(false);
    // } 
}