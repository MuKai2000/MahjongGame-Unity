using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mahjong_Server;
using mahjong;
using UnityEngine.SceneManagement;
// using System.Thread;

public enum States{Init, NotReady, Ready, GetTile, LayTile, Operate, Others, End};
public enum Wind{East, South, West, North};

public class Player{
    public int id = -1;
    public string name = "Player";
    public Wind wind = Wind.East;
    public int w = 0;
    public int who = -1;

    public Player(){}
    public Player(int id_in, string name_in, Wind wind_in, int w_in){
        id = id_in;
        name = name_in;
        wind = wind_in;
        w = w_in; 
    }
};

public class GameControl : MonoBehaviour{
    Client client = new Client();
    // 房间信息    
    private string RoomNumber = "0";
    int num_players = 0;
    private List<Player> players_order = new List<Player>();
    private List<Player> players = new List<Player>();   
    // 个人信息
    private string self_player_name = "a Player";   // 自己的名字     
    // 对局信息
    private string[] pai = new string[136]; // 牌 GameObject
	public bool include_red_tiles = true;   //    
    private Wind self_wind;  // 自风
    private List<int> self_player_hand = new List<int>();   // 手牌
    private List<int> self_player_desk = new List<int>();   // 
    private int self_player_get =  -1 ;    // 摸得牌
    private int turn = 0; // 0 自己 1 上家 2 对家 3 下家

    private List<int> GameOperation = new List<int>();
    private List<string> up_hand = new List<string>();
    private List<string> po_hand = new List<string>();
    private List<string> down_hand = new List<string>();
    private Vector3 myPos = new Vector3(0f,0f,0f);
    private Vector3 upPos = new Vector3(0f,0f,0f);
    private Vector3 poPos = new Vector3(0f,0f,0f);
    private Vector3 downPos = new Vector3(0f,0f,0f);
    private Vector3 myPosLayout = new Vector3(1f,0f,1.5f);
    private Vector3 upPosLayout = new Vector3(1.5f,0f,-1f);
    private Vector3 poPosLayout = new Vector3(-1f,0f,-1.5f);
    private Vector3 downPosLayout = new Vector3(-1.5f,0f,1f);
    private Vector3 myPosDesk_Ro = new Vector3(0f,0f,0f);
    private Vector3 upPosDesk_Ro = new Vector3(0f,90f,0f);
    private Vector3 poPosDesk_Ro = new Vector3(0f,180f,0f);
    private Vector3 downPosDesk_Ro = new Vector3(0f,270f,0f);
    private Vector3 myPosDesk = new Vector3(2f,0f,4.5f);
    private Vector3 upPosDesk = new Vector3(4.5f,0f,-2f);
    private Vector3 poPosDesk = new Vector3(-2f,0f,-4.5f);
    private Vector3 downPosDesk = new Vector3(-4.5f,0f,2f);   
    private int[] Recorder = new int[34]; // 记牌器 条0-8 饼9-17 万18-26 东南西北27-30 中发白31-33
    private int index_PaiShan = 0;  // 指明牌山位置
    private States state = States.Init; // 棋局状态
    private int  last = -1;
    // 服务器信息
    private string GetMsg;
    private string SendMsg;
    int test = 0;
    // Start is called before the first frame update
    void Start(){   
        PlayerPrefs.GetInt("OpenModel");
        //text();
        // test
        Hu hu = new Hu();
        Debug.Log("Hello World");
        Debug.Log(state);
        ConnectServer();
        
        InitDesk();
        InitData();
        SendMsg = "setName_"+self_player_name;
        SendMsgToServer();
        Debug.Log("-------------------------"+PlayerPrefs.GetInt("OpenModel").ToString()+"-------------------------------" );
        if(PlayerPrefs.GetInt("OpenModel")==-1)
            SendMsg = "Create";
        else
            SendMsg = "Join_" + PlayerPrefs.GetInt("OpenModel").ToString();
        // GameObject.Find("North_Chicha").GetComponent<WindTag>().Move(new Vector3(0,0,0));
        SendMsgToServer();  // 发送消息
        // Debug.Log(client.getsss());
    }
    // Update is called once per frame
    void Update(){   
        List<int> newHand = new List<int>();
        for(int i=0;i<self_player_hand.Count;i++){
            if(self_player_hand[i]!=-1)
                newHand.Add(self_player_hand[i]);
        }
        self_player_hand = newHand;



        // 状态 1 NotReady 2 Ready 3 接受牌 4 打牌 5 吃碰胡 6 他人操作       0 结束--> NotReady

        if(state == States.NotReady){    // 未准备 等待就绪点击
            // 点击准备按钮 进入准备状态
            int hasReady = GameObject.Find("Button_Ready").GetComponent<MyButton>().ifClicked();
            // Debug.Log(hasReady);
            if(hasReady==1){
                // 出现 取消准备按钮
                GameObject.Find("Button_Ready").GetComponent<MyButton>().Disappear();
                GameObject.Find("Button_CancelReady").GetComponent<MyButton>().Show();
                // 向服务器发送 “准备信息”
                SendMsg = "Ready";
                SendMsgToServer();
                // 进入 准备状态
                state = States.Ready;
                Debug.Log(state);
                GameObject.Find("Button_Ready").GetComponent<MyButton>().ChangeClicked(0);
            }
        }
        else if(state == States.Ready){     // 准备状态 
            // 点击取消准备 回到 NotReady
            int hasCancelReady = GameObject.Find("Button_CancelReady").GetComponent<MyButton>().ifClicked();
            // Debug.Log(hasCancelReady);
            if(hasCancelReady==1){
                // 出现 取消准备按钮
                GameObject.Find("Button_CancelReady").GetComponent<MyButton>().Disappear();
                GameObject.Find("Button_Ready").GetComponent<MyButton>().Show();
                // 向服务器发送 “取消准备信息”
                SendMsg = "CancelReady";
                SendMsgToServer();
                // 进入 准备状态
                state = States.NotReady;
                Debug.Log(state);
                GameObject.Find("Button_CancelReady").GetComponent<MyButton>().ChangeClicked(0);
            }

            // 对局是否开始
            int hasBegin = 0;
            // 接受服务器开始消息 进入对局状态
            GetMsgFromServer();
            string[] msg = GetMsg.Split('_');
            if(GetMsg!="")
                Debug.Log(GetMsg);
            // 处理服务器信息   对局开始 发牌起始位置 东偏x 手牌序列
            if(msg.Length>1){
                Debug.Log("msg" + msg[0]);
                if(msg[0] == "PlayerWind"){ // 玩家信息
                    players_order.Add(new Player(int.Parse(msg[1]),msg[2],Wind.East,0));
                    players_order.Add(new Player(int.Parse(msg[4]),msg[5],Wind.South,0));
                    players_order.Add(new Player(int.Parse(msg[7]),msg[8],Wind.West,0));
                    players_order.Add(new Player(int.Parse(msg[10]),msg[11],Wind.North,0));

                    for(int i=0;i<4;i++){
                        // Debug.Log("++++++++" + StringWind(players_order[i].wind));
                        Debug.Log(players_order[i].w);
                    }

                    int pos = int.Parse(msg[13]);
                    index_PaiShan = pos*2 ;

                    // Debug.Log("----------------------Test3-----------------------");
                    // 
                }
                else if(msg[0] == "GameStart"){ // 手牌
                    Debug.Log(GetMsg);
                    // 自风
                    Debug.Log(msg[1]);
                    if(msg[1]=="East"){
                        self_wind = Wind.East;
                        index_PaiShan = index_PaiShan ;
                    }
                    else if(msg[1]=="West"){
                        self_wind = Wind.West;
                        index_PaiShan = index_PaiShan + 17*2*2;
                        }
                    else if(msg[1]=="South")
                        {self_wind = Wind.South;
                        index_PaiShan = index_PaiShan + 17*2*1;
                        }
                    else
                        {self_wind = Wind.North;
                        index_PaiShan = index_PaiShan + 17*2*3;
                        }
                    for(int i = 2;i<15;i++){
                        self_player_hand.Add(int.Parse(msg[i]));
                        self_player_desk.Sort();
                    }
                    Debug.Log("index:   " + index_PaiShan.ToString());
                    hasBegin = 1;

                }
                test++;
            }
            
            if(hasBegin==1){    // 发牌
                // 信息
                // Debug.Log("----------------------Test5-----------------------");
                GameObject.Find("Button_CancelReady").GetComponent<MyButton>().Disappear();
                InitGame();
                // Debug.Log("----------------------Test10-----------------------");

                if(self_wind == Wind.East){// 东
                    state = States.GetTile;
                    Debug.Log(state);
                }
                else{// 其他
                    state = States.Others;
                    Debug.Log(state);
                }
            }
            
            
        }
        else if(state == States.GetTile){
            turn = 0;
            // 接受服务器发牌消息
            GetMsgFromServer();
            string[] msg = GetMsg.Split('_');
            if(msg.Length > 1){
                Debug.Log("GetTile::"+ GetMsg);
                if(msg[0] =="GetPai"){  // 摸牌
                    GameObject obj = GameObject.Find(pai[index_PaiShan]);   // 新牌
                    self_player_get = int.Parse(msg[1]);
                    int type = self_player_get / 4;
                    Recorder[type]--;
                    Vector3 newV = myPos;
                    newV.x -= 0.3f;
                    obj.GetComponent<Pai>().ChangePai("Pai_" + self_player_get.ToString(), newV, new Vector3(50f,0f,0f) , self_player_get);
                    index_PaiShan = (index_PaiShan+1)%136;

                    for(int i=0;i<self_player_hand.Count;i++){
                        Debug.Log(self_player_hand[i]);
                    }
                    Debug.Log(self_player_get);
                    for(int i=0;i<self_player_desk.Count;i++){
                        Debug.Log(self_player_desk[i]);
                    }
                    // 棋牌逻辑判断
                    List<int> Operation = new List<int>();

                    List<int> text=new List<int>(self_player_hand);
                    text=Hu.Transform1(text);
                    int text2=Hu.Transform3(self_player_get);
                    List<int> text_=new List<int>(self_player_desk);
                    text_=Hu.Transform1(text_);
                    if(Hu.IsCanHU(text,text2,text_)){//判断是否胡牌
                        Operation.Add(5);
                    }
                    if(Hu.stop(text,text2,text_)){//手牌，听
                        Operation.Add(4);
                    }
                    if(Hu.Bar(text,text2)){//手牌，杠
                        Operation.Add(3);
                    }
                    GameOperation = Operation;


                    // int Operation = 0;  // 3 杠 4 听 5 胡
                    // 如果存在 吃碰胡
                    if(GameOperation.Count != 0){   // 显示按钮
                        GameOperation.Sort();
                        for(int i=0;i<GameOperation.Count;i++){
                            switch(GameOperation[i]){
                                case 1: GameObject.Find("Button_Chi").GetComponent<MyButton>().Show();break;
                                case 2: GameObject.Find("Button_Peng").GetComponent<MyButton>().Show();break;
                                case 3: GameObject.Find("Button_Gang").GetComponent<MyButton>().Show();break;
                                case 4: GameObject.Find("Button_Ting").GetComponent<MyButton>().Show();break;
                                case 5: GameObject.Find("Button_Hu").GetComponent<MyButton>().Show();break;
                            }
                        }
                        GameObject.Find("Button_Pass").GetComponent<MyButton>().Show();
                        state = States.Operate; // 选择操作
                        Debug.Log(state);
                    }
                    else{
                        state = States.LayTile;     // 打牌
                        Debug.Log(state);
                    }
                }
            }
             
        }
        else if(state == States.LayTile){
            turn = 0;
            self_player_hand.Add(self_player_get);
            self_player_get = -1;
            self_player_hand.Sort();                   
            if(Input.GetMouseButtonDown(0)){
                GameObject obj = hasClicked("Pai");
                Debug.Log("Clicked:"+ obj.name);
                int inhand = 0;
                for(int i=0;i<self_player_hand.Count;i++){
                    if(self_player_hand[i] == int.Parse(obj.name.Split('_')[1])){
                        inhand = 1;
                        break;}
                }
                if(obj != GameObject.Find("table") && inhand == 1){
                    obj.GetComponent<Pai>().Move(myPosLayout);
                    obj.GetComponent<Transform>().rotation = Quaternion.Euler(myPosDesk_Ro.x,myPosDesk_Ro.y,myPosDesk_Ro.z);

                    // 调整位置
                    myPosLayout.x = myPosLayout.x - 0.3f;
                    if(myPosLayout.x < -1f){
                        myPosLayout.x = 1f;
                        myPosLayout.z = myPosLayout.z + 0.4f;
                    }
                    
                    int no = int.Parse(obj.name.Split('_')[1]);
                    List<int> new_hand = new List<int>();
                        
                    for(int i=0;i<self_player_hand.Count;i++){
                        if(self_player_hand[i] != no)
                            new_hand.Add(self_player_hand[i]);
                    }

                    new_hand.Sort();
                    self_player_hand = new_hand;
                    
                    FreshHand();
                    
                    SendMsg = "LayPai_" + no.ToString();  // 发牌信息
                    SendMsgToServer();

                    SendMsg = "NoOp";      // 无其他操作
                    SendMsgToServer();

                    state = States.Others;
                    Debug.Log(state);
                }    
            }
        }
        else if(state == States.Operate){
            // 1 吃 2 碰 3 杠 4 听 5 胡
            if(GameObject.Find("Button_Chi").GetComponent<MyButton>().ifClicked() == 1 ){
                ButtonDis();
                List<int> text_1=new List<int> (self_player_hand);
                text_1=Hu.Transform1(text_1);
                int text_2=Hu.Transform3(self_player_get);
                List<int> text_bet=new List<int>();         //  可以吃的牌类型
                text_bet=Hu.Order_eat(text_1,text_2);//返回手牌
                
                    
                int One = text_bet[0];
                int Two = text_bet[1];
                One = Hu.Transform4(One);
                Two = Hu.Transform4(Two);
                int has1=0,has2=0;
                for(int i=0;i<self_player_hand.Count;i++){
                    if(self_player_hand[i]/4 == One&& has1==0){
                        One = self_player_hand[i];
                        has1=1;
                    }
                    if(self_player_hand[i]/4 == Two&&has2==0){
                        Two = self_player_hand[i];
                        has2=1;
                    }
                }
                
                SendMsg = "OpChi_" + One.ToString() + "_"+ Two.ToString() +"_"+ self_player_get.ToString();
                SendMsgToServer();

                

                self_player_hand.Add(self_player_get);
                self_player_get = -1;
                
                state = States.Others;
                //=Hu.Make_eat(text_1,text_2,text_bet,nnnnnnn);
            }else if(GameObject.Find("Button_Peng").GetComponent<MyButton>().ifClicked()==1){
                ButtonDis();
                List<int> text_1=new List<int> (self_player_hand);
                text_1=Hu.Transform1(text_1);
                int text_2=Hu.Transform3(self_player_get);
                text_1=Hu.Order_touch(text_1,text_2);//返回手牌

                int One = text_2;
                One = Hu.Transform4(One);
                List<int> m = new List<int>();
                int has1=0;

                self_player_hand.Add(self_player_get);
                self_player_get = -1;

                for(int i=0;i<self_player_hand.Count;i++){
                    if(self_player_hand[i]/4 == One && has1<3){
                        m.Add(self_player_hand[i]);
                        has1++;
                    }
                }
                m.Add(self_player_get);
                SendMsg = "OpPeng";
                for(int i =0;i<3;i++){
                    SendMsg = SendMsg + "_" + m[i].ToString();
                }
                SendMsgToServer();
                state = States.Others;
                Debug.Log(state);
            }else if(GameObject.Find("Button_Gang").GetComponent<MyButton>().ifClicked()==1){
                ButtonDis();
                List<int> text_1=new List<int> (self_player_hand);
                text_1=Hu.Transform1(text_1);
                int text_2=Hu.Transform3(self_player_get);
                text_1=Hu.Order_Bar(text_1,text_2);//返回手牌

                int One = text_2;
                One = Hu.Transform4(One);
                List<int> m = new List<int>();
                int has1=0;

                self_player_hand.Add(self_player_get);
                self_player_get = -1;

                for(int i=0;i<self_player_hand.Count;i++){
                    if(self_player_hand[i]/4 == One&& has1<4){
                        m.Add(self_player_hand[i]);
                        has1++;
                    }
                }
                m.Add(self_player_get);
                GetMsg = "OpGang_";
                for(int i =0;i<4;i++){
                    SendMsg = SendMsg + "_" + m[i].ToString();
                }
                SendMsgToServer();
                state = States.Others;
                Debug.Log(state);
            }else if(GameObject.Find("Button_Ting").GetComponent<MyButton>().ifClicked()==1){
                ButtonDis();
                List<int> text_1=new List<int> (self_player_hand);
                text_1=Hu.Transform1(text_1);
                int text_2=Hu.Transform3(self_player_get);
                List<int> text_=new List<int> (self_player_desk);
                text_=Hu.Transform1(text_);              
                int pai = Hu.stop_Order(text_1,text_2,text_);//返回手牌
                pai = Hu.Transform4(pai);
                int has =0;
                for(int i=0;i<self_player_hand.Count;i++){
                    if(pai == self_player_hand[i]/4&&has==0){

                        pai = self_player_hand[i];
                        has = 1;        
                    }
                        
                }
                // ---------------------------------------------------------------------
                SendMsg = "OpTing_" + pai.ToString();
                SendMsgToServer();

                self_player_hand.Add(self_player_get);
                self_player_get = -1;
                
                state = States.Others;
                Debug.Log(state);
            }else if(GameObject.Find("Button_Hu").GetComponent<MyButton>().ifClicked()==1){
                ButtonDis();/*
                List<int> text_1=new List<int> (self_player_hand);
                text_1=Hu.Transform1(text_1);
                int text_2=Hu.Transform3(self_player_get);
                Hu.IsCanHU(text_1,text_2);*/

                SendMsg = "OpHu_";
                self_player_hand.Add(self_player_get);
                for(int i=0;i<self_player_hand.Count;i++){
                    if(self_player_hand[i]!=-1)
                        GetMsg = GetMsg + "_" + self_player_hand[i].ToString();
                }
                for(int i=0;i<self_player_desk.Count;i++){
                    if(self_player_desk[i]!=-1)
                        GetMsg = GetMsg + "_" + self_player_desk[i].ToString();
                }
                SendMsgToServer();
            }else if(GameObject.Find("Button_Pass").GetComponent<MyButton>().ifClicked()==1){
                ButtonDis();
                if(self_player_get != -1){
                    state = States.LayTile;
                    Debug.Log(state);
                }
                else{
                    SendMsg = "NoOp";
                    SendMsgToServer();
                }
            }


        }
        else if(state == States.Others){
            GetMsgFromServer();
            string[] msg = GetMsg.Split('_');
            if(msg.Length!=0){

                if(msg[0] == "PlayerLayPai"){
                    int p = int.Parse(msg[2]);
                    Recorder[p/4]--;
                    GameObject obj = GameObject.Find(pai[index_PaiShan]);
                    obj.name = "Pai_"+p.ToString();
                    last = p;
                    obj.GetComponent<Pai>().ChangeMetrial(p);
                    index_PaiShan = (index_PaiShan+1)%136;
                    switch(msg[1]){
                        case "East": turn = WHO(Wind.East);break;
                        case "South":turn  = WHO(Wind.South);break;
                        case "West":turn = WHO(Wind.West);break;
                        case "North":turn = WHO(Wind.North);break;
                    }
                    // turn = WHO(w);
                    if(turn == 1){
                        obj.GetComponent<Pai>().Move(upPosLayout);
                        obj.GetComponent<Transform>().rotation = Quaternion.Euler(upPosDesk_Ro.x,upPosDesk_Ro.y,upPosDesk_Ro.z);
                        upPosLayout.z = upPosLayout.z + 0.3f;
                        if(upPosLayout.z > 1.0f){
                            upPosLayout.z = -1f;
                            upPosLayout.x += 0.4f;
                        }

                    }else if(turn == 2){
                        obj.GetComponent<Pai>().Move(poPosLayout);
                        obj.GetComponent<Transform>().rotation = Quaternion.Euler(poPosDesk_Ro.x,poPosDesk_Ro.y,poPosDesk_Ro.z);
                        poPosLayout.x = poPosLayout.x + 0.3f;
                        if(upPosLayout.z > 1.0f){
                            poPosLayout.z -= -0.4f;
                            poPosLayout.x = -1f;
                        }

                    }else if(turn == 3){
                        obj.GetComponent<Pai>().Move(downPosLayout);
                        obj.GetComponent<Transform>().rotation = Quaternion.Euler(downPosDesk_Ro.x,downPosDesk_Ro.y,downPosDesk_Ro.z);
                        downPosLayout.z = downPosLayout.z - 0.3f;
                        if(downPosLayout.z <-1.0f){
                            downPosLayout.z = 1f;
                            downPosLayout.x -= 0.5f;
                        }
                    }else if(turn == 0){
                    }
                    // 逻辑判断
                    self_player_get = p;

                    for(int i=0;i<self_player_hand.Count;i++){
                        Debug.Log(self_player_hand[i]);
                    }
                    Debug.Log(self_player_get);
                    for(int i=0;i<self_player_desk.Count;i++){
                        Debug.Log(self_player_desk[i]);
                    }

                    List<int> Operate=new List<int>();
                    List<int> text_1=new List<int>(self_player_hand);
                    text_1=Hu.Transform1(self_player_hand);
                    int text_2=Hu.Transform3(self_player_get);
                    List<int> text_3=new List<int>(self_player_desk);
                    text_3=Hu.Transform1(self_player_desk);
                    //
                    if(turn == 1){
                        // 上家吃
                        if(Hu.Eat(text_1,text_2)==true){
                            Operate.Add(1);
                        }
                    }
                    if(Hu.Touch(text_1,text_2)==true){
                        Operate.Add(2);
                    }
                    if(Hu.Bar(text_1,text_2)==true){
                        Operate.Add(3);
                    }
                    if(Hu.IsCanHU(text_1,text_2,text_3)==true){
                        Operate.Add(5);
                    }
                    GameOperation=Operate;

                    if(GameOperation.Count != 0){   // 显示按钮
                        GameOperation.Sort();
                        for(int i=0;i<GameOperation.Count;i++){
                            switch(GameOperation[i]){
                                case 1: GameObject.Find("Button_Chi").GetComponent<MyButton>().Show();break;
                                case 2: GameObject.Find("Button_Peng").GetComponent<MyButton>().Show();break;
                                case 3: GameObject.Find("Button_Gang").GetComponent<MyButton>().Show();break;
                                case 4: GameObject.Find("Button_Ting").GetComponent<MyButton>().Show();break;
                                case 5: GameObject.Find("Button_Hu").GetComponent<MyButton>().Show();break;
                            }
                        }
                        GameObject.Find("Button_Pass").GetComponent<MyButton>().Show();
                        state = States.Operate; // 选择操作
                        Debug.Log(state);
                    }
                    else{
                        SendMsg = "NoOp";
                        SendMsgToServer();
                    }
                }
                else if(msg[0] == "PlayerNoOp"){    
                    switch(msg[1]){
                        case "East": turn = WHO(Wind.East);break;
                        case "South":turn  = WHO(Wind.South);break;
                        case "West":turn = WHO(Wind.West);break;
                        case "North":turn = WHO(Wind.North);break;
                    }
                    if(turn == 0){
                        state = States.GetTile;
                        Debug.Log(state);
                    }
                }
                else if(msg[0] == "PlayerOpChi" || msg[0] == "PlayerOpPeng" || msg[0] == "PlayerOpGang" || msg[0] == "PlayerOpTing"){
                    switch(msg[1]){
                        case "East": turn = WHO(Wind.East);break;
                        case "South":turn  = WHO(Wind.South);break;
                        case "West":turn = WHO(Wind.West);break;
                        case "North":turn = WHO(Wind.North);break;
                    }
                    int num = msg.Length - 2;
                    List<int> tiles = new List<int>();
                    for(int i=2; i< msg.Length;i++)
                        tiles.Add(int.Parse(msg[i]));
                    if(turn == 0){
                        // 自家
                        self_player_hand.Add(self_player_get);  // 把别人打的牌或发给你的牌
                        self_player_get = -1;
                        List<int> newL = new List<int>();
                        for(int i = 0;i<self_player_hand.Count;i++){    //把打出的牌从手中去除
                            int tag = 0;
                            for(int j=0;j<tiles.Count;j++){
                                if(self_player_hand[i]==tiles[j])
                                    tag = 1;
                            }
                            if(tag == 0 && self_player_hand[i]!=-1)
                                newL.Add(self_player_hand[i]);
                        }
                        self_player_hand = newL;
                        FreshHand();
                        for(int i=0;i<tiles.Count;i++){
                            self_player_desk.Add(tiles[i]);
                            GameObject obj = GameObject.Find("Pai_"+tiles[i].ToString());
                            obj.GetComponent<Transform>().rotation = Quaternion.Euler(myPosDesk_Ro.x,myPosDesk_Ro.y,myPosDesk_Ro.z);
                            if(msg[0] == "PlayerOpTing"){
                                obj.GetComponent<Pai>().Move(myPosLayout);
                                myPosLayout.x -= 0.3f;
                                if(myPosLayout.x < -1f){
                                    myPosLayout.z += 0.4f;
                                    myPosLayout.x = 1f;
                                }
                            }else{
                                obj.GetComponent<Pai>().Move(myPosDesk);
                                myPosDesk.x -= 0.3f;
                                self_player_desk.Add(tiles[i]);
                            }
                            
                        }

                        if(msg[0] == "PlayerOpGang"){
                            state = States.GetTile;
                            Debug.Log(state);
                        }
                        else{
                            state = States.LayTile;
                            Debug.Log(state);
                        }

                    }
                    else if(turn == 1){
                        // 上家
                        int tag = 0;
                        for(int i=0;i<tiles.Count;i++){
                            if(last == tiles[i]){
                                tag = 1;
                                // tile.Remove(tile[i]);
                                break;
                            }
                        }
                        List<string> T = new List<string>();

                        int tip = 0;
                        for(int i=0;i<tiles.Count-1;){
                            if(up_hand[tip]!=""){
                                T.Add(up_hand[tip]);
                                up_hand[tip] = "";
                                i++;
                            }
                            tip++;
                        }
                        if(tag == 0){   // 牌山
                            T.Add(pai[index_PaiShan]);
                            index_PaiShan = (index_PaiShan +1) %136;
                        }
                        else{   // 牌河
                            T.Add("Pai_"+last.ToString());
                        }

                        // T 里是对象
                        for(int i = 0;i<tiles.Count;i++){
                            GameObject obj = GameObject.Find(T[i]);
                            obj.name = "Pai_" + tiles[i].ToString();
                            obj.GetComponent<Pai>().ChangeMetrial(tiles[i]);
                            obj.GetComponent<Transform>().rotation = Quaternion.Euler(upPosDesk_Ro.x,upPosDesk_Ro.y,upPosDesk_Ro.z);
                            if(msg[0] == "PlayerOpTing"){
                                obj.GetComponent<Pai>().Move(upPosLayout);
                                upPosLayout.z = upPosLayout.z + 0.3f;
                                if(upPosLayout.z > 1.0f){
                                    upPosLayout.z = -1f;
                                    upPosLayout.x += 0.4f;
                                }
                            }else{
                                obj.GetComponent<Pai>().Move(upPosDesk);
                                upPosDesk.z = upPosDesk.z + 0.3f;
                            }
                        }
                        state = States.Others;
                        Debug.Log(state);
                    }
                    else if(turn == 2){
                        // 对家
                        int tag = 0;
                        for(int i=0;i<tiles.Count;i++){
                            if(last == tiles[i]){
                                tag = 1;
                                // tile.Remove(tile[i]);
                                break;
                            }
                        }
                        List<string> T = new List<string>();

                        int tip = 0;
                        for(int i=0;i<tiles.Count-1;){
                            if(up_hand[tip]!=""){
                                T.Add(up_hand[tip]);
                                up_hand[tip] = "";
                                i++;
                            }
                            tip++;
                        }
                        if(tag == 0){   // 牌山
                            T.Add(pai[index_PaiShan]);
                            index_PaiShan = (index_PaiShan +1) %136;
                        }
                        else{   // 牌河
                            T.Add("Pai_"+last.ToString());
                        }

                        // T 里是对象
                        for(int i = 0;i<tiles.Count;i++){
                            GameObject obj = GameObject.Find(T[i]);
                            obj.name = "Pai_" + tiles[i].ToString();
                            obj.GetComponent<Pai>().ChangeMetrial(tiles[i]);
                            obj.GetComponent<Transform>().rotation = Quaternion.Euler(poPosDesk_Ro.x,poPosDesk_Ro.y,poPosDesk_Ro.z);

                            obj.GetComponent<Pai>().Move(poPosLayout);
                            if(msg[0] == "PlayerOpTing"){
                                obj.GetComponent<Pai>().Move(poPosLayout);
                                poPosLayout.x = poPosLayout.x + 0.3f;
                                if(poPosLayout.x > 1.0f){}
                                poPosLayout.z = poPosLayout.z - 0.3f;
                                poPosLayout.x = -1f;
                            }else{
                                obj.GetComponent<Pai>().Move(poPosDesk);
                                poPosDesk.x = poPosDesk.x + 0.3f;
                            }
                        }
                        state = States.Others;
                        Debug.Log(state);

                    }
                    else if(turn == 3){
                        // 
                        int tag = 0;
                        for(int i=0;i<tiles.Count;i++){
                            if(last == tiles[i]){
                                tag = 1;
                                // tile.Remove(tile[i]);
                                break;
                            }
                        }
                        List<string> T = new List<string>();

                        int tip = 0;
                        for(int i=0;i<tiles.Count-1;){
                            if(up_hand[tip]!=""){
                                T.Add(up_hand[tip]);
                                up_hand[tip] = "";
                                i++;
                            }
                            tip++;
                        }
                        if(tag == 0){   // 牌山
                            T.Add(pai[index_PaiShan]);
                            index_PaiShan = (index_PaiShan +1) %136;
                        }
                        else{   // 牌河
                            T.Add("Pai_"+last.ToString());
                        }

                        // T 里是对象
                        for(int i = 0;i<tiles.Count;i++){
                            GameObject obj = GameObject.Find(T[i]);
                            obj.name = "Pai_" + tiles[i].ToString();
                            obj.GetComponent<Pai>().ChangeMetrial(tiles[i]);
                            
                            obj.GetComponent<Transform>().rotation = Quaternion.Euler(downPosDesk_Ro.x,downPosDesk_Ro.y,downPosDesk_Ro.z);
                            if(msg[0] == "PlayerOpTing"){
                                obj.GetComponent<Pai>().Move(downPosLayout);
                                downPosLayout.z = downPosLayout.z - 0.3f;
                                if(downPosLayout.z <-1.0f){
                                downPosLayout.z = 1f;
                                downPosLayout.x -= 0.5f;
                            }else{
                                obj.GetComponent<Pai>().Move(downPosDesk);
                                downPosDesk.z = downPosDesk.z - 0.3f;
                            }
                            }
                            
                            
                            
                        }
                        state = States.Others;  
                        Debug.Log(state);              
                    }
                    SendMsg = "NoOp";
                    SendMsgToServer();
          
        
                } 
                else if(msg[0] == "PlayerOpHu"){
                    Debug.Log(msg);
                    GameObject.Find("Button_Win").GetComponent<MyButton>().Show();
                    GameObject.Find("Button_Win").GetComponent<MyButton>().ChangeTxt(msg[1]);
                    state = States.End;
                }
            }
            else if(state == States.End){
                
                if(GameObject.Find("Button_Win").GetComponent<MyButton>().ifClicked()==1){
                    state = States.NotReady;
                    GameObject.Find("Button_Win").GetComponent<MyButton>().Disappear();
                    GameObject.Find("Button_Win").GetComponent<MyButton>().ChangeClicked(0);
                    InitDesk();
                    InitData();
                }

                // Start();
            }
        }

        // 对局执行
            // 接受服务器信息
            // 控制牌
            // 接受玩家操作
            // 控制牌
            // 发送信息至服务器

    }
    private void ButtonDis(){
        GameObject.Find("Button_Chi").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Peng").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Gang").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Ting").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Hu").GetComponent<MyButton>().Disappear();
        GameObject.Find("Button_Pass").GetComponent<MyButton>().Disappear();

        GameObject.Find("Button_Chi").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Peng").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Gang").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Ting").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Hu").GetComponent<MyButton>().ChangeClicked(0);
        GameObject.Find("Button_Pass").GetComponent<MyButton>().ChangeClicked(0);
    }
    private void ConnectServer(){
        client.StartClient();
    }
    private void SendMsgToServer(){
        client.sendMessage(System.Type.GetType("Mahjong_Server.Information"), new Information(self_player_name,SendMsg));
        //Thread.Sleep(5);
    }
    private void GetMsgFromServer(){
        GetMsg = client.getMessage();
    }
    private GameObject Instant(string name, Vector3 position, Vector3 rotation){
        GameObject pre = (GameObject)Resources.Load("MyPrefabs/Pai");
        GameObject instance = (GameObject)Instantiate(pre, position, new Quaternion(0f,0f,0f,0f));
        Material Mater = Resources.Load<Material>("Material/jihai_ haku");
        instance.GetComponent<Renderer>().material = Mater;
        instance.name = name;
        instance.transform.Rotate(rotation);
        return instance;
    }
    private void InitDesk(){    // 初始化桌面
        // 生成排队序列
        Vector3 position = new Vector3(0f, 0f, 0f);
        Vector3 rotation = new Vector3(0f, 0f, 0f);
        for(int i=0; i<68; i++){
            if(i == 0){  // 本家
                position = new Vector3(-2.7f,0.5f,3f);
                rotation = new Vector3(0f, 0f, -180f);}
            else if(i == 17){    // 上家
                position = new Vector3(3f,0.5f,2.7f);
                rotation = new Vector3(0f, 90f, -180f);}

            else if(i == 34){    // 对家
                position = new Vector3(2.7f,0.5f,-3f);
                rotation = new Vector3(-180f, 0f, 0f);}

            else if(i == 51){    // 下家
                position = new Vector3(-3f,0.5f,-2.7f);
                rotation = new Vector3(0f, -90f, -180f);}

            
            for(int j=0;j<2;j++){
                int no = i*2+j;
                string pai_name = "Clone_"+no.ToString();
                GameObject new_Pai = Instant(pai_name, position, rotation);
                pai[no] = new_Pai.name;
                position.y -= 0.25f;
            }

            position.y = 0.5f;  // 回复第二层坐标
            if(i>=0 && i<17)
                position.x += 0.3f;
            else if(i>=17 && i<34)
                position.z -= 0.3f;
            else if(i>=34 && i<51)
                position.x -= 0.3f;
            else if(i>=51)
                position.z += 0.3f;
        }
        
        // 显示 就绪按钮
        GameObject.Find("Button_Ready").GetComponent<MyButton>().Show();
        // 初始化记牌器
        for(int i =0;i<34;i++){
            Recorder[i] = 4;
        }
        // 状态改变
        state = States.NotReady;    // 改变状态为 未就绪
        Debug.Log(state);
    }
    private int WHO(Wind wind){ // 1 上家 2 对家 3 下家
        Debug.Log("----------------------6.6-----------------------");
        switch (self_wind)
        {
            case Wind.East:{
                switch (wind)
                {
                    case Wind.South: return 3;break;
                    case Wind.West: return 2;break;
                    case Wind.North: return 1;break;                    
                }
                break;
            }
            case Wind.South:{
                switch (wind)
                {
                    case Wind.East: return 1;break;
                    case Wind.West: return 3;break;
                    case Wind.North: return 2;break;                    
                }
                break;
            }
            case Wind.West:{
                switch (wind)
                {
                    case Wind.South: return 1;break;
                    case Wind.East: return 2;break;
                    case Wind.North: return 3;break;                    
                }
                break;
            }
            case Wind.North:{
                switch (wind)
                {
                    case Wind.South: return 2;break;
                    case Wind.West: return 1;break;
                    case Wind.East: return 3;break;                    
                }
                break;
            }
        }
        return 0;
    }
    private string StringWind(Wind wind){
        if(wind == Wind.East)
            return "East";
        else if(wind == Wind.South)
            return "South";
        else if(wind == Wind.West)
            return "West";
        else
            return "North";
    }
    private void InitGame(){
        // 风牌
        for(int i = 0;i<4;i++){
            Debug.Log("----------------------Test6-----------------------"+i.ToString());
            players_order[i].who = WHO(players_order[i].wind);  // 判断方位
            Debug.Log("----------------------Test7-----------------------"+i.ToString());
            Debug.Log(players_order[i].who);
            if(players_order[i].who == 0){
                GameObject.Find(StringWind(players_order[i].wind)).GetComponent<WindTag>().Move(new Vector3(0f,0f,1f));
                GameObject.Find(StringWind(players_order[i].wind)).transform.Rotate(0,0,0);
                Debug.Log("----------------------Test8-----------------------"+i.ToString());
                Vector3 pos = new Vector3(2f,0.2f,3.5f);
                for(int j =0; j<13 ; j++){
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Pai>().Move(pos);
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Transform>().rotation = Quaternion.Euler(50f,0f,0f);
                    pos.x -= 0.3f;
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Pai>().ChangeMetrial(self_player_hand[j]);
                    int type = self_player_hand[j] / 4;
                    Recorder[type]--;
                    GameObject.Find(pai[index_PaiShan]).name = "Pai_" + self_player_hand[j].ToString();
                    Debug.Log("----------------------Test9-----------------------"+i.ToString());
                    index_PaiShan = (index_PaiShan+1)%136;
                }
                myPos = pos;
            }
            else if(players_order[i].who == 1){
                GameObject.Find(StringWind(players_order[i].wind)).GetComponent<WindTag>().Move(new Vector3(1f,0f,0f));
                GameObject.Find(StringWind(players_order[i].wind)).transform.Rotate(0,90,0);
                Vector3 pos = new Vector3(3.5f,0.2f,-2f);
                for(int j =0; j<13 ; j++){
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Pai>().Move(pos);
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Transform>().rotation = Quaternion.Euler(-90f,0f,-90f);
                    up_hand.Add(GameObject.Find(pai[index_PaiShan]).name);
                    pos.z += 0.3f;
                    index_PaiShan = (index_PaiShan+1)%136;
                }
                upPos = pos;
                
            }
            else if(players_order[i].who == 2){
                Debug.Log("----------------------Test50-----------------------"+i.ToString());
                GameObject.Find(StringWind(players_order[i].wind)).GetComponent<WindTag>().Move(new Vector3(0f,0f,-1f));
                GameObject.Find(StringWind(players_order[i].wind)).transform.Rotate(0,180,0);
                Vector3 pos = new Vector3(-2f,0.2f,-3.5f);
                Debug.Log("----------------------Test51-----------------------"+i.ToString());
                for(int j =0; j<13 ; j++){
                    Debug.Log(index_PaiShan);

                    GameObject.Find(pai[index_PaiShan]).GetComponent<Pai>().Move(pos);
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Transform>().rotation = Quaternion.Euler(-90f,0f,0f);
                    po_hand.Add(GameObject.Find(pai[index_PaiShan]).name);
                    pos.x += 0.3f;
                    index_PaiShan = (index_PaiShan+1)%136;
                }
                Debug.Log("----------------------Test52-----------------------"+i.ToString());
                poPos = pos;
            }
            else if(players_order[i].who == 3){
                GameObject.Find(StringWind(players_order[i].wind)).GetComponent<WindTag>().Move(new  Vector3(-1f,0f,0f));
                GameObject.Find(StringWind(players_order[i].wind)).transform.Rotate(0,270,0);
                Vector3 pos = new Vector3(-3.5f,0.2f,2f);
                for(int j =0; j<13 ; j++){
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Pai>().Move(pos);
                    GameObject.Find(pai[index_PaiShan]).GetComponent<Transform>().rotation = Quaternion.Euler(90f,-90f,0);
                    down_hand.Add(GameObject.Find(pai[index_PaiShan]).name);
                    pos.z -= 0.3f;
                    index_PaiShan = (index_PaiShan+1)%136;
                }
                downPos = pos;
            }
        }

    }
    private void InitData(){
        Player p = new Player();
        for(int i=0;i<4;i++){
            // players_order.Add(p);
            players.Add(p);
            //players_order[i]=p;
            //players[i]=p;
        }        
    }
    private GameObject hasClicked(string KeyWords){
        GameObject res = GameObject.Find("table");
        
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        RaycastHit[] array = Physics.RaycastAll( ray );
        RaycastHit hit;
        Transform tf;
        if(array.Length != 0){
            for(int i = 0;i<array.Length;i++){
                GameObject obj = array[i].transform.gameObject;
                Debug.Log(obj.name);
            } 
            for(int i = 0;i<array.Length;i++){
                GameObject obj = array[i].transform.gameObject;
                if(obj.name.Split('_')[0] == KeyWords){
                    res =  obj;
                }
            } 
        }
        return res;
    }
    private void FreshHand(){
        Vector3 pos = new Vector3(2f,0.2f,3.5f);
            for(int j =0; j<self_player_hand.Count ; j++){
                if(self_player_hand[j] != -1){
                    GameObject obj = GameObject.Find("Pai_"+self_player_hand[j].ToString());
                    obj.GetComponent<Pai>().Move(pos);
                    obj.GetComponent<Transform>().rotation = Quaternion.Euler(50f,0f,0f);
                    pos.x -= 0.3f;
                }
            }
            myPos = pos;
        
    }
}
