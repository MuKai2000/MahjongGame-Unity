using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyButton : MonoBehaviour
{
    Vector3 pos;
    // Start is called before the first frame update
    int hasClicked = 0;
    void Start(){
        pos = this.gameObject.transform.position;
        Disappear();

    }
    // Update is called once per frame
    void Update(){}
    public void ChangeTxt(string msg){
        Text text = this.GetComponent<Button>().transform.Find("Text").GetComponent<Text>(); //------------(2)
        //或者吧（1）（2）合并成：
        //Text text = GameObject.Find("填写button名/Text").getComponent<Text>();
        text.text= msg + "is Win!"; //此方法用于改变控件的文本值
    }
    public void Click(){
        Debug.Log("has Clicked!");
        hasClicked = 1;
    }
    public int ifClicked(){
        return hasClicked;
    }
    public void ChangeClicked(int tag){
        hasClicked = tag;
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
