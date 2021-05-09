using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pai : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){ }
    // Update is called once per frame
    void Update(){    }
    public bool hasClicked(){
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit ;    
        if (Physics.Raycast (ray, out hit)) { 
            return true;
        }else{
            return false;
        }
    }
    public void Move(Vector3 pos){
        this.gameObject.transform.position = pos;
    }
    public void ChangePai(string nameV, Vector3 pos, Vector3 rota, int no){
        this.name = nameV;
        Move(pos);
        ChangeMetrial(no);
        this.GetComponent<Transform>().rotation = Quaternion.Euler(rota.x,rota.y,rota.z);
    }
    public void ChangeMetrial(int no){
        int type = no / 4;
        string path = "Material/";
        switch (type)
        {
            case 0: path += "souzu_1" ;break;
            case 1: path += "souzu_2" ;break;
            case 2: path += "souzu_3" ;break;
            case 3: path += "souzu_4" ;break;
            case 4: path += "souzu_5" ;break;
            case 5: path += "souzu_6" ;break;
            case 6: path += "souzu_7" ;break;
            case 7: path += "souzu_8" ;break;
            case 8: path += "souzu_9" ;break;
            case 9: path += "pinzu_1" ;break;
            case 10: path += "pinzu_2" ;break;
            case 11: path += "pinzu_3" ;break;
            case 12: path += "pinzu_4" ;break;
            case 13: path += "pinzu_5" ;break;
            case 14: path += "pinzu_6" ;break;
            case 15: path += "pinzu_7" ;break;
            case 16: path += "pinzu_8" ;break;
            case 17: path += "pinzu_9" ;break;
            case 18: path += "manzu_1" ;break;
            case 19: path += "manzu_2" ;break;
            case 20: path += "manzu_3" ;break;
            case 21: path += "manzu_4" ;break;
            case 22: path += "manzu_5" ;break;
            case 23: path += "manzu_6" ;break;
            case 24: path += "manzu_7" ;break;
            case 25: path += "manzu_8" ;break;
            case 26: path += "manzu_9" ;break;
            case 27: path += "jihai_ton" ;break;
            case 28: path += "jihai_nan" ;break;
            case 29: path += "jihai_sha" ;break;
            case 30: path += "jihai_pe" ;break;
            case 31: path += "jihai_chun" ;break;
            case 32: path += "jihai_hatsu" ;break;
            case 33: path += "jihai_haku" ;break;
        }
        Material Mater = Resources.Load<Material>(path);
        this.GetComponent<Renderer>().material = Mater;
    }
}
