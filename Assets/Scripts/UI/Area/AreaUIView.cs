using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AreaUIView : MonoBehaviour
{
    public Button levelBtn1;
    public Button levelBtn2;
    public Button returnBtn;
    public Button confirmBtn;
    public Text workText;
    public Text rewardText;
    

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

    public void ConfirmBtnEnable()
    {
        confirmBtn.interactable = true;
    }

    public void ConfirmBtnDisable()
    {
        confirmBtn.interactable = false;
    }

    /// <summary>
    /// 通过name获取子物体gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject GetChildGameObject(string name)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var childTransform = transform.GetChild(i);
            if (childTransform.name != name)
            {
                continue;
            }

            return childTransform.gameObject;
        }

        Debug.LogError(name + "Cannot Find In Child GameObject");
        return null;
    }
}