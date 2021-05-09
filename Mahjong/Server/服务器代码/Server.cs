using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Collections.Concurrent;

namespace Mahjong_server
{
    /*
    interface TypeToInt
    {
        public int typeToInt();
    }
    class Test :AutoSerializable<Test>,TypeToInt
    {
        public int a {get;set;}
        public string s { get; set; }
        public Test(int a,string s="123")
        {
            this.a = a;
            this.s = s + "\n";
        }
        public void print()
        {
            Console.WriteLine("out");
        }
        public Test convert(Object obj)
        {
            return (Test)obj;
        }
        
        public Test(String s)
        {
            Test t = JsonSerializer.Deserialize<Test>(s);
            this.a = t.a;
            this.s = t.s;
        }

        public Test Deserialize(String s)
        {
            Test t = JsonSerializer.Deserialize<Test>(s);
            return t;
        }

        public Test() { }

        public int typeToInt()
        {
            return 1;
        }
    }
    */
    abstract class Server
    {
        /*服务器*/
        // size of send buffer
        public const int BufferSize = 1024;
        // P V
        ManualResetEvent allDone = new ManualResetEvent(false);
        //线程安全的消息队列
        public ConcurrentQueue<Message> messageQueue = new ConcurrentQueue<Message>();
        //int和类型的对照表
        public ConcurrentDictionary<int, Type> typeDict = new ConcurrentDictionary<int, Type>();
        // ID 和 player的对照表
        public ConcurrentDictionary<int, Player> players = new ConcurrentDictionary<int, Player>();

        public string backID;

        internal Message Message
        {
            get => default;
            set
            {
            }
        }

        internal Player Player
        {
            get => default;
            set
            {
            }
        }

        protected Type intToType(int typeInt)
        {
            return typeDict[typeInt];
        }

        protected int typeToInt(Type type, Object obj)
        {
            /*类型转int*/
            MethodInfo methodInfo = type.GetMethod("typeToInt");
            return (int)methodInfo.Invoke(obj, null);
        }

        abstract public void pushQueue(int playerID, Message message);
        abstract protected bool checkPlayerIsOnline(int playerID);

        abstract public void playerOffLine(int playerID);
        public void sendMessage(Type type, Object message,Socket socket)
        {
            /*发送消息，对象和类型*/
            /*类型的int编码，json长度，json*/
            Console.WriteLine(((Information)message).a + " " + ((Information)message).s);
            // send buffer
            byte[] buffer = new byte[BufferSize];
            string s = JsonSerializer.Serialize(message);
            // socket.Send()
            byte[] typeByte = BitConverter.GetBytes(typeToInt(type, message));
            byte[] lengthByte = BitConverter.GetBytes(s.Length);
            byte[] jsonByte = System.Text.Encoding.ASCII.GetBytes(s);
            for (int i = 0; i < 4; i++)
            {
                buffer[i+4] = typeByte[i];
                buffer[i] = lengthByte[i];
            }
            for (int i = 0; i < jsonByte.Length; i++)
            {
                buffer[i + 8] = jsonByte[i];
            }
            //移到缓冲区
            try
            {
                socket.Send(buffer, s.Length + 8, 0);
                //发送所有数据
            }
            catch (Exception e)
            {
                Console.WriteLine("Send message error !");
                Console.WriteLine(e.Message);
            }
            Thread.Sleep(5);
        }

        protected bool parseMessage(int playerID, byte[] data,int now,int len,ref byte[]last,ref int used)
        {
            /*解析消息，并放入消息队列*/
            if (used != 0)
            {
                //分包，后半段
                //先保存到last中
                int last_len = BitConverter.ToInt32(last, now+0);
                int i;
                for (i = 0; i < len && used < last_len; used++, i++, now++) 
                {
                    last[used] = data[now];
                }
                if (used == last_len)
                {
                    //正好相等，说明此时包已经完整
                    int temp = 0;
                    byte[] b_temp = new byte[1024];
                    parseMessage(playerID, last, 0, used, ref b_temp, ref temp);
                    used = 0;
                    return parseMessage(playerID, data, now, len - i, ref last, ref used);
                }
                else
                {
                    //包不完整，继续等待
                    return true;
                }
            }
            int typeInt = BitConverter.ToInt32(data,now+4);
            int length = BitConverter.ToInt32(data, now+0);
            if (len < 8)
            {
                return false;
            }
            if (len < length + 8)
            {
                //分包，前半部分
                //保存下来
                for (int i = 0; i < len; used++, i++) 
                {
                    last[used] = data[now + i];
                }
                return true;
            }
            string json = System.Text.Encoding.ASCII.GetString(data, now+8, length);
            // 获取类型，构造对象
            Type type = intToType(typeInt);
            Object obj = (Object)Activator.CreateInstance(type, json);
            // 放入消息队列
            if (((Information)obj).a=="a Player")
            {
                Console.WriteLine("RECEIVE "+((Information)obj).a + " " + ((Information)obj).s);
            }
            pushQueue(playerID, new Message(type, obj, playerID));
            len = len - length - 8;
            if (len > 0)
            {
                return parseMessage(playerID, data, now + length + 8, len,ref last,ref used);
            }
            return true;
        }

        protected void receiveMessage(Object objplayer)
        {
            Player player = objplayer as Player;
            int playerID = player.getID();
            byte[] data = new byte[BufferSize];
            byte[] last = new byte[BufferSize];
            int used = 0;
            int length = 8;
            int messageLength = 0;
            //总长度
            while (checkPlayerIsOnline(playerID))
            {
                //同步接受消息
                try
                {
                    messageLength = player.socket.Receive(data);
                    length = BitConverter.ToInt32(data, 4) + 8;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Player {0} offLine!", player.getID());
                    playerOffLine(playerID);
                    break;
                }
                //消息接受不完整，玩家掉线
                if (messageLength < length)
                {
                    Console.WriteLine("Player {0} offLine!", player.getID());
                    playerOffLine(playerID);
                    break;
                }
                try
                {
                    // 解析消息并且放入消息队列中
                    if(!parseMessage(playerID, data,0,messageLength,ref last,ref used))
                    {
                        break;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("Message resolve error");
                    Console.WriteLine(e.Message);
                }
            }
        }

        abstract protected void resolveAndCallback(object message);

        public void broadcast(Type type, Object message)
        {
            /*广播函数，向全体玩家发送*/
            foreach(Player player in players.Values)
            {
                sendMessage(type, message, player.socket);
            }
        }
    }
}
