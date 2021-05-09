using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mahjong_server
{
    class RoomServer : Server
    {
        public int roomID;
        static int IDCNT = 0;
        // 房间是否存活
        public bool roomFlag = true;
        // 玩家数量
        protected const int PlayerSizeMax = 4;
        public static LobbyServer lobby = null;
        // 游戏进行中标志
        protected object _lock = new object();
        protected bool gaming = false;
        Game game;
        int gameCnt = 0;
        public override void playerOffLine(int playerID)
        {
            lock (_lock)
            {
                Player player;
                players[playerID].offLine();
                players.TryRemove(playerID, out player);
                if (gaming)
                {
                    //正在游戏中
                    gameEndWithError();
                }
            }
        }

        public override void pushQueue(int playerID, Message message)
        {
            messageQueue.Enqueue(message);
        }

        protected override bool checkPlayerIsOnline(int playerID)
        {
            bool isOnline;
            isOnline = players[playerID].online();
            return isOnline;
        }

        public void setRoomID(int ID)
        {
            /*设置房间ID*/
            roomID = ID;
        }

        public int getRoomID()
        {
            return roomID;
        }

        public RoomServer()
        {
            roomID = IDCNT++;
            lobby = LobbyServer.getInstance();
        }

        public void run()
        {
            /*运行函数*/
            while (roomFlag)
            {
                while (!messageQueue.IsEmpty && roomFlag)
                {
                    // 队列非空，取出消息
                    Message message;
                    messageQueue.TryDequeue(out message);
                    resolveAndCallback(message);
                }
                Thread.Sleep(0);
            }
        }


        protected override void resolveAndCallback(object message)
        {
            Message mess = (Message)message;
            Information information = (Information)mess.obj;
            int playerID = mess.ID;
            string[] s = information.s.Split("_");
            string pai = "";
            if (s.Length > 2)
            {
                for (int i = 1; i < s.Length; i++)
                {
                    if (i != 1)
                    {
                        pai += "_";
                    }
                    pai += s[i];
                }
            }
            backID = information.a;
            try
            {
                lock (_lock)
                {
                    if (!gaming)
                    {
                        switch (s[0])
                        {
                            case "exit":
                                {
                                    // 退出房间
                                    playerExitRoomCallBack(players[playerID]);
                                    break;
                                }
                            case "CancelReady":
                                {
                                    //取消准备
                                    playerNotReadyCallBack(playerID);
                                    break;
                                }
                            case "Ready":
                                {
                                    // 准备
                                    playerReadyCallBack(playerID);
                                    break;
                                }
                            default:
                                {
                                    //复读测试
                                    string str = "Hello World! " + information.a + " " + information.s + " --- " + "in " + "room " + roomID + " " + players.Count + " / " + PlayerSizeMax;
                                    Information res = new Information(-1, str);
                                    sendMessage(res.GetType(), res, players[mess.ID].getSocket());
                                    break;
                                }
                        }
                    }
                    else
                    {
                        // 游戏内
                        switch (s[0])
                        {
                            case "LayPai":
                                {
                                    layPaiCallBack(playerID, Convert.ToInt32(s[1]));
                                    break;
                                }
                            case "NoOp":
                                {
                                    NoOpCallBack(playerID, s[0]);
                                    break;
                                }
                            case "OpTing": 
                                {
                                    OpTingCallBack(playerID, Convert.ToInt32(s[1]));
                                    break;
                                }
                            case "OpChi":
                                {
                                    OpChiCallBack(playerID, pai);
                                    break;
                                }
                            case "OpPeng":
                                {
                                    OpPengCallBack(playerID, pai);
                                    break;
                                }
                            case "OpGang":
                                {
                                    OpGangCallBack(playerID, pai);
                                    break;
                                }
                            case "OpHu":
                                {
                                    OpHuCallBack(playerID, pai);
                                    break;
                                }
                            default:
                                {
                                    //TODO
                                    //复读测试
                                    string str = "Hello World! " + information.a + " " + information.s + " --- " + "in " + "room " + roomID + " " + players.Count + " / " + PlayerSizeMax;
                                    Information res = new Information(-1, str);
                                    sendMessage(res.GetType(), res, players[mess.ID].getSocket());
                                    break;
                                }
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(playerID + " message error! " + information.s);
            }
            return;
            throw new NotImplementedException();
        }

        public void start()
        {
            //房间开始运行
            run();
        }


        public bool isFull()
        {
            /*房间是否满员*/
            return players.Count >= PlayerSizeMax;
        }

        public void insertPlayer(Player player)
        {
            /*房间内加入一个玩家*/
            players.TryAdd(player.getID(), player);
            players[player.getID()].enterRoom(roomID);
            players[player.getID()].setNotReady();
        }

        public void playerExitRoomCallBack(Player player)
        {
            /*玩家退出房间回调*/
            gameCnt = 0;
            lobby.playExitRoom(player.getID());
            players.TryRemove(player.getID(), out player);
            //玩家为空 房间销毁
            if (players.Count == 0)
            {
                lobby.deleteRoom(roomID);
                roomFlag = false;
            }
            //发送回返消息
            string s = backID + " exit " + roomID;
            Information res = new Information(-1, s);
            sendMessage(res.GetType(), res, player.getSocket());
            //显示
            Console.WriteLine(s);
            return;
            // TODO
            throw new NotImplementedException();
        }

        protected void playerNotReadyCallBack(int playerID)
        {
            /*取消准备的回调*/
            players[playerID].setNotReady();


            // 广播
            Information inf = new Information(-1, "PlayerCancelReady" + "_" + players[playerID].getUserName());
            broadcast(inf.GetType(), inf);
        }

        protected void playerReadyCallBack(int playerID)
        {
            /*玩家准备的回调*/
            players[playerID].setReady();

            // 广播就绪
            Information inf = new Information(-1, "PlayerReady" + "_" + players[playerID].getUserName());
            broadcast(inf.GetType(), inf);

            // 查询准备人数
            int readyCnt = 0;
            foreach(Player p in players.Values)
            {
                if (p.getReady())
                {
                    readyCnt++;
                }
            }

            if (readyCnt == PlayerSizeMax && players.Count == PlayerSizeMax)
            {
                //准备人数到达游戏人数上限
                //游戏开始
                startGame();
            }
        }

        protected void startGame()
        {
            /*游戏开始函数*/
            // 第一场游戏随机，之后的游戏场数轮风
            if (gameCnt == 0)
            {
                gaming = true;
                List<int> temp = new List<int>();
                foreach (Player ptemp in players.Values)
                {
                    temp.Add(ptemp.getID());
                }
                game = new Game(temp);
            }
            else
            {
                gaming = true;
                game.nextGame();
            }



            // 游戏开始
            List<int> p = game.getPlayerIDs();
            List<int>[] pai = game.getPlayerPai();

            //广播，排风
            string feng = "PlayerWind";
            for (int i = 0; i < p.Count; i++)
            {
                feng = feng + "_" + players[p[i]].getUserName() + "_" + game.trans[i];
            }
            Information information = new Information(-1, feng+"_"+game.getRand());
            broadcast(information.GetType(), information);

            // 初始发牌
            for (int i = 0; i < p.Count; i++)
            {
                String s = game.trans[i];
                for(int j = 0; j < pai[i].Count; j++)
                {
                    s = s + "_" + pai[i][j];
                }
                Information inf = new Information(-1, "GameStart" + "_" + s);
                sendMessage(inf.GetType(), inf, players[p[i]].getSocket());
            }
            // 给东风发牌
            Thread.Sleep(10);
            getPai();
        }

        protected void gameEndWithError()
        {
            /*游戏异常结束*/
            gaming = false;
            gameCnt = 0;
            foreach(Player p in players.Values)
            {
                // 所有玩家取消准备
                playerNotReadyCallBack(p.getID());
            }
        }

        public string getAllPlayerUserName()
        {
            string res = "";
            foreach(Player p in players.Values)
            {
                res = res + "_" + p.getUserName();
            }
            return res;
        }

        protected void getPai()
        {
            /*摸牌*/
            /*
            Console.WriteLine(game.size());
            if (game.empty())
            {
                // 没有牌堆，流牌
                Information information = new Information(-1, "");
                Console.WriteLine("finish!");
                broadcast(information.GetType(), information);
                gameOver(-1);
                return;
            }*/
            //发给风
            Information inf = new Information(-1, "GetPai" + "_" + game.getNextPai());
            Console.WriteLine(players[game.getNowPlayerID()].getUserName() + " " + inf.s);
            sendMessage(inf.GetType(), inf, players[game.getNowPlayerID()].getSocket());
        }

        protected void layPaiCallBack(int playerID,int pai)
        {
            /*出牌回调*/

            // 特殊操作置位
            game.setOpFlag();

            //广播所有人，出牌情况
            Information inf = new Information(-1, "PlayerLayPai" + "_" + game.getNowFeng() + "_" + pai);
            broadcast(inf.GetType(), inf);
            
        }

        protected void NoOpCallBack(int playerID,string sOp)
        {
            /*无操作的判断符号*/
            if (game.getSOpFlag())
            {
                game.addSOp(playerID, sOp);

                if (game.sOpEnd())
                {
                    //所有人的消息都已经得到
                    sOpEnd();
                }
            }
        }

        protected void OpTingCallBack(int playerID,int pai)
        {
            /*听牌回调*/

            // 特殊操作置位
            game.setOpFlag();

            //广播所有人，出牌情况
            Information inf = new Information(-1, "PlayerOpTing" + "_" + game.getNowFeng() + pai);
            broadcast(inf.GetType(), inf);
        }

        protected void OpChiCallBack(int playerID,string pai)
        {
            /*吃牌回调*/
            if (game.getSOpFlag())
            {
                //放入特殊操作队列里
                game.addSOp(playerID, "OpChi", pai);

                if (game.sOpEnd())
                {
                    //所有人的消息都已经得到
                    sOpEnd();
                }
            }
        }

        protected void sOpEnd()
        {
            /*特殊操作结束的广播*/
            int playerID; 
            string op; 
            string pai;
            game.getSOpEnd(out playerID, out op, out pai);
            //清空
            game.opClear();

            
            if (op == "NoOp") {
                //没有操作，顺位到下一个人，广播，摸牌
                game.nextPlayerID();
                Information inf = new Information(-1, "PlayerNoOp" + "_" + game.getNowFeng());
                broadcast(inf.GetType(), inf);
                getPai();
            }else if (op == "OpHu")
            {
                //广播
                Information inf = new Information(-1, "PlayerOpHu" + "_" + game.getPlayerFeng(playerID) + "_" + pai);
                broadcast(inf.GetType(), inf);

                gameOver(playerID);
            }
            else
            {
                //没有胡牌，调整下一位出牌的人
                game.setNowPlayerID(playerID);
                //广播
                Information inf = new Information(-1, "Player" + op + "_" + game.getPlayerFeng(playerID) + "_" + pai);
                broadcast(inf.GetType(), inf);
                
                //只有杠摸牌，吃、碰不摸牌
                if (op == "OpGang")
                {
                    getPai();
                }
            }

        }

        protected void gameOver(int PlayerID)
        {
            /*游戏正常结束*/
            gameCnt++;
            gaming = false;
            game.over(PlayerID);
            foreach (Player p in players.Values)
            {
                // 所有玩家取消准备
                playerNotReadyCallBack(p.getID());
            }
        }

        protected void OpPengCallBack(int playerID,string pai)
        {
            /*碰牌回调*/
            if (game.getSOpFlag())
            {
                //放入特殊操作队列里
                game.addSOp(playerID, "OpPeng", pai);

                if (game.sOpEnd())
                {
                    //所有人的消息都已经得到
                    sOpEnd();
                }
            }
        }

        protected void OpGangCallBack(int playerID,string pai)
        {
            /*杠牌回调*/
            if (game.getSOpFlag())
            {
                //放入特殊操作队列里
                game.addSOp(playerID, "OpGang", pai);

                if (game.sOpEnd())
                {
                    //所有人的消息都已经得到
                    sOpEnd();
                }
            }
            else
            {
                game.setNowPlayerID(playerID);
                //广播
                Information inf = new Information(-1, "Player" + "OpGang" + "_" + game.getPlayerFeng(playerID) + "_" + pai);
                broadcast(inf.GetType(), inf);
                getPai();

            }
        }

        protected void OpHuCallBack(int playerID, string pai)
        {
            if (game.getSOpFlag())
            {
                //放入特殊操作队列里
                game.addSOp(playerID, "OpHu", pai);

                if (game.sOpEnd())
                {
                    //所有人的消息都已经得到
                    sOpEnd();
                }
            }
            else
            {
                //广播
                Information inf = new Information(-1, "PlayerOpHu" + "_" + game.getPlayerFeng(playerID) + "_" + pai);
                broadcast(inf.GetType(), inf);

                gameOver(playerID);
            }
        }

        internal Game Game
        {
            get => default;
            set
            {
            }
        }
    }
}
