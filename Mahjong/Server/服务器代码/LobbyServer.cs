using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mahjong_server
{
    class LobbyServer : Server
    {
        // 房间
        public ConcurrentDictionary<int, RoomServer> rooms = new ConcurrentDictionary<int, RoomServer>();
        // 大厅socket
        public Socket lobbyServerSocket;
        // 玩家上限
        protected const int PlayersSizeMax = 20;
        // 端口
        protected const int endPoint = 33224;

        // private static string ip = "172.17.69.122";
        private const string ip = "172.22.122.19";

        private static LobbyServer instance = null;
        public static LobbyServer getInstance()
        {
            if (instance == null)
            {
                instance = new LobbyServer();
                instance.init();
            }
            return instance;
        }
        private LobbyServer() { }
        public override void playerOffLine(int playerID)
        {
            bool isRoom;
            int roomID = 0;
            Player player;
            players[playerID].offLine();
            isRoom = players[playerID].isInRoom();
            if (isRoom)
            {
                roomID = players[playerID].getRoomID();
                // 房间执行掉线函数
                rooms[roomID].playerOffLine(playerID);
            }

            players.TryRemove(playerID, out player);
        }

        protected override bool checkPlayerIsOnline(int playerID)
        {
            // 是否在线
            bool isOnline;
            isOnline = players[playerID].online();
            return isOnline;
            
        }

        public override void pushQueue(int playerID, Message message)
        {
            bool isRoom;
            int roomID = 0;
            // 查询是否在房间内
            isRoom = players[playerID].isInRoom();
            if (isRoom)
            {
                roomID = players[playerID].getRoomID();
            }
            if (isRoom)
            {
                // 在房间内，消息加入房间内的队列
                rooms[roomID].pushQueue(playerID, message);
            }
            else
            {
                // 不在房间，消息加入大厅内的队列
                messageQueue.Enqueue(message);
            }
        }

        public void listen()
        {
            /*监听端口，接受连接并且转入线程*/
            Socket socket;
            try
            {
                while (true) {
                    // 同步等待连接
                    socket = lobbyServerSocket.Accept();
                    // 生成新的玩家
                    Player player = new Player(socket);
                    // 加入玩家队列
                    players.TryAdd(player.getID(), player);

                    Console.WriteLine("{0} join game lobby!", player.getID());

                    // 创建线程监听
                    ParameterizedThreadStart receiveMethod = new ParameterizedThreadStart(receiveMessage);

                    Thread receive = new Thread(receiveMessage) { IsBackground = true };

                    receive.Start(player);
                }
            } catch (Exception e)
            {
                Console.WriteLine("listen error!");
                Console.WriteLine(e.Message);
            }
        }

        public void run()
        {
            /*处理消息队列*/
            while (true)
            {
                while (!messageQueue.IsEmpty)
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
            /*解析消息*/
            Message mess = (Message)message;
            Information information = (Information)mess.obj;
            int playerID = mess.ID;
            backID = information.a;
            try
            {
                string[] s = information.s.Split("_");
                if (s[0] == "Create")
                {
                    // 创建房间
                    int roomID=createRoomCallBack();
                    plyaerEnterRoomCallBack(playerID, roomID);
                    Console.WriteLine(information.a + " " + "Create " + roomID);
                }
                else if (s[0] == "Join")
                {
                    // 进入房间
                    int roomID = Convert.ToInt32(s[1]);
                    plyaerEnterRoomCallBack(playerID, roomID);
                }else if (s[0] == "setName")
                {
                    players[playerID].setName(s[1]);
                    Console.WriteLine(playerID + " setName " + s[1]);
                }
                else
                {
                    Console.WriteLine("again! " + information.a + " " + information.s);
                    //复读测试
                    string str = "Hello World! " + information.a + " " + information.s + " --- " + "in " + "lobby";
                    Information res = new Information(-1, str);
                    sendMessage(res.GetType(), res, players[mess.ID].getSocket());
                }
            }catch(Exception e)
            {
                Console.WriteLine(playerID+" message error! "+information.s);
            }
            return;
            // TODO
            throw new NotImplementedException();
        }

        public string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName(); //得到主机名
            IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    return iPEntry.AddressList[i].ToString();
            }
            return null;
        }
        public void init()
        {
            typeDict.TryAdd(1, typeof(Information));
            // ip = GetLocalIPv4();
            // ip = "172.17.69.122";
            lobbyServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), endPoint);
            lobbyServerSocket.Bind(point);
            lobbyServerSocket.Listen(PlayersSizeMax);

            Console.WriteLine(ip + " " + endPoint + " start!");
            
            // 大厅启动
            Thread runThread = new Thread(new ThreadStart(run));
            runThread.Start();

            Console.WriteLine("lobbyServer start!");
            //启动监听
            listen();
            //退出
            lobbyServerSocket.Close();
        }

        public int createRoomCallBack()
        {
            /*创建房间回调*/
            RoomServer room = new RoomServer();
            int ID = room.getRoomID();
            rooms.TryAdd(ID, room);
            Thread roomThread = new Thread(new ParameterizedThreadStart(createRoom));
            roomThread.Start(ID);
            return ID;
        }

        public void createRoom(object roomID)
        {
            /*创建房间*/
            int ID = (int)roomID;
            rooms[ID].start();
            deleteRoom(ID);
        }

        public void deleteRoom(int roomID)
        {
            /*删除房间*/
            RoomServer temp;
            rooms.TryRemove(roomID, out temp);
            // 输出房间被销毁
            Console.WriteLine(roomID + " delete !");
        }

        public void plyaerEnterRoomCallBack(int playerID,int roomID)
        {
            /*玩家进入房间回调*/
            string s= backID + " enter " + roomID;
            try
            {
                if (rooms[roomID].isFull())
                {
                    // 已经满员
                    s = "RoomFull";
                    Information errorRes = new Information(-1, "RoomFull");
                    sendMessage(errorRes.GetType(), errorRes, players[playerID].getSocket());
                    Console.WriteLine(roomID + " " + s);
                    return;
                }
                players[playerID].enterRoom(roomID);
                rooms[roomID].insertPlayer(players[playerID]);

                // 广播，有人进入
                Information information = new Information(-1, "PlayerJoin" + "_" + playerID + "_" + players[playerID].getUserName());
                rooms[roomID].broadcast(information.GetType(), information);

                //向进入者发送目前房间内已有的人
                Information roomPlayer = new Information(-1, "PlayerIn" + rooms[roomID].getAllPlayerUserName());
                sendMessage(roomPlayer.GetType(), roomPlayer, players[playerID].getSocket());

                Console.WriteLine(s);
            }catch(Exception e)
            {
                s += " error! room is not exit!";
                Information errorRes = new Information(-1, "RoomFull");
                sendMessage(errorRes.GetType(), errorRes, players[playerID].getSocket());
            }
        }

        public void playExitRoom(int playerID)
        {
            /*玩家退出房间*/
            //int roomID = players[playerID].getRoomID();
            players[playerID].exitRoom();
            return;
            // TODO
            throw new NotImplementedException();
        }
    }
}
