using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyInput : MonoBehaviour
{
    private string valueText = "";
    private string endValue = "";
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;
        Disappear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangedValue(string value)
    {
        valueText=value;//将用户输入的值赋值给内部的空字符串，我们可以将其来进行后续的操作
        Debug.Log("输入了"+value);
    }
    public void EndValue(string value)
    {
        endValue=value;//捕捉数据，方便后续操作
        Debug.Log("最终内容"+value);
    }

    public void Disappear(){
        Move(new Vector3(-9000f,-9000f,-9000f));
    }

    public void Show(){
        Move(pos);
    }

    public void setPos(Vector3 pos_new){
        pos = pos_new;
    }

    private void Move(Vector3 pos){
        this.gameObject.transform.position = pos;
    }
}
