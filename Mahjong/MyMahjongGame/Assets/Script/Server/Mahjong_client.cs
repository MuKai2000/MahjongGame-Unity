using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections.Concurrent;
namespace Mahjong_Server
{
    class Message
    {
        public Type type;
        public System.Object obj;

        public Message() { }
        public Message(Type type,System.Object obj)
        {
            this.type = type;
            this.obj = obj;
        }
    }

    class Client{
        //const string ip = "47.110.154.93";
        const string ip = "172.22.122.19";
        const int endPoint = 33224;
        public static Socket socket;
        public string sss = "";
        
        
        public ConcurrentQueue<Information> messageQueue = new ConcurrentQueue<Information>();

        public void Disconnected(){
            try{
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }catch(Exception e){

            }
        }

        public void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, endPoint);

                // Create a TCP/IP  socket.  
                socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    socket.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        socket.RemoteEndPoint.ToString());

                    Thread lis = new Thread(new ParameterizedThreadStart(receiveMessage)) { IsBackground = true };
                    lis.Start(socket);
                    // Thread runthread = new Thread(new ThreadStart(run));
                    // runthread.Start();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        protected static Type intToType(int typeInt)
        {
            if (typeInt == 1)
            {
                return typeof(Information);
            }
            else
            {
                return typeof(string);
            }
        }

        protected static int typeToInt(Type type, System.Object obj)
        {
            /*类型转int*/
            MethodInfo methodInfo = type.GetMethod("typeToInt");
            return (int)methodInfo.Invoke(obj, null);
        }
        public void sendMessage(Type type, System.Object message)
        {
            /*发送消息，对象和类型*/
            /*类型的int编码，json长度，json*/
            // send buffer
            byte[] buffer = new byte[1024];
            //string s = JsonUtility.ToJson(message);
            string s= "{\"a\":\""+((Information)message).a+"\",\"s\":\""+((Information)message).s+"\"}";
            sss = s;
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
        }

        protected bool parseMessage(int playerID, byte[] data,int now,int len , ref byte[] last, ref int used)
        {
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
            /*解析消息，并放入消息队列*/
            int typeInt = BitConverter.ToInt32(data, now+4);
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
            // Debug.Log("adfkhaj;dlghao "+json);
            Debug.Log("JSON:::"+json);
            // 获取类型，构造对象
            string[] jsons = json.Split('\"');
            Type type = intToType(typeInt);
            //System.Object obj = (System.Object)Activator.CreateInstance(type, json);
            Information obj = new Information(jsons[3],jsons[7]);
            
            // 放入消息队列
            messageQueue.Enqueue(obj);
			len = len - length - 8;
            if (len > 0)
            {
                return parseMessage(playerID, data, now + length + 8, len, ref last, ref used);
            }
            return true;
        }

        protected void receiveMessage(System.Object objplayer)
        {
            byte[] last = new byte[1024];
            int used = 0;
            Socket socket = objplayer as Socket;
            byte[] data = new byte[1024];
            int messageLength = 0;
            int length = 0;
            //总长度
            while (true)
            {
                //同步接受消息
                try
                {
                    messageLength = socket.Receive(data);
                    length = BitConverter.ToInt32(data, 4) + 8;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
                //消息接受不完整，玩家掉线
                if (messageLength < length)
                {
                    Console.WriteLine("Player {0} offLine!", 555);
                    Console.ReadLine();
                    System.Environment.Exit(0);
                }
                // 解析消息并且放入消息队列中
                if(!parseMessage(0, data,0,messageLength, ref last,ref used)){
                    Debug.Log("ERROR! Mess");
					continue;
				}
            }
            try{
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }catch(Exception e){

            }
            
        }


        public void run()
        {
            while (true)
            {
                while (!messageQueue.IsEmpty)
                {
                    Information message;
                    messageQueue.TryDequeue(out message);
                    string test = message.s;
                    string s = "Server message! " + test + "\n";
                    Console.WriteLine(s);
                }
                Thread.Sleep(0);
            }
        }

        public string getMessage(){
            string s;
            // Debug.Log(messageQueue.IsEmpty);
            if(!messageQueue.IsEmpty){
                Information message=new Information();
                messageQueue.TryDequeue(out message);
                s = message.s;
                Debug.Log("s:" + s);
            }else{
                s="";
            }
            return s;
        }

        public string getsss(){
            return sss;
        }
    }
}

