using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HallControl : MonoBehaviour
{
    private int state = 0;
    private string valueText = "";
    private string endValue = "";
    // InputField input = InputField.Find("Input_Room");
    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update()
    {   
        if(state==0){
            GameObject.Find("Button_Create").GetComponent<MyButton>().Show();
            GameObject.Find("Button_Join").GetComponent<MyButton>().Show();
            GameObject.Find("Button_Enter").GetComponent<MyButton>().Disappear();
            GameObject.Find("Input_Room").GetComponent<MyInput>().Disappear();
            if(GameObject.Find("Button_Create").GetComponent<MyButton>().ifClicked()==1)
                state = 1;
            else if(GameObject.Find("Button_Join").GetComponent<MyButton>().ifClicked()==1)
                state = 2;
        }
        else if(state==1){
            ChangePage();
            PlayerPrefs.SetInt("OpenModel", -1);
            SceneManager.LoadScene("Game");
        }
        else if(state == 2){
            ChangePage();
            GameObject.Find("Button_Enter").GetComponent<MyButton>().Show();
            GameObject.Find("Input_Room").GetComponent<MyInput>().Show();
            // transform.GetComponent<InputField>().onValueChanged.AddListener(ChangedValue);
            GameObject.Find("Input_Room").GetComponent<InputField>().onEndEdit.AddListener(EndValue);
            if(GameObject.Find("Button_Enter").GetComponent<MyButton>().ifClicked()==1){
                ChangePage();
                PlayerPrefs.SetInt("OpenModel", int.Parse(endValue));
                SceneManager.LoadScene("Game");
            }
        }
    }
    public void ChangePage(){
        GameObject.Find("Button_Create").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Create").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Join").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Join").GetComponent<MyButton>().Disappear();
    }
    private void ChangedValue(string value)
    {
        valueText=value;//将用户输入的值赋值给内部的空字符串，我们可以将其来进行后续的操作
        Debug.Log("输入了"+value);
    }
    private void EndValue(string value)
    {
        endValue=value;//捕捉数据，方便后续操作
        Debug.Log("最终内容"+value);
    }
}
