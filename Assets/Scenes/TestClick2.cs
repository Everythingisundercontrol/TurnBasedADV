using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClick2 : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public Text text;


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)Camera.main.transform.position).normalized;

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, direction);

            var log = "";
            foreach (RaycastHit2D hit in hits)
            {
                log += "Hit " + hit.collider.gameObject.name + "\n";
            }

            text.text = log;
        }
    }


    public void Change2()
    {
        obj2.SetActive(false);
        obj2.SetActive(true);
    }
    
    public void Change1()
    {
        obj1.SetActive(false);
        obj1.SetActive(true);
    }
}
