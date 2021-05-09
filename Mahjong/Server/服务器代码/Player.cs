using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Mahjong_server
{
    class Player
    {
        static int IDCNT = 0;
        //socket
        public Socket socket;
        // 是否在线
        protected bool onlineFlag;
        // 玩家ID
        protected int ID;
        // 是否在房间内
        public bool inRoom;
        // 所处房间ID
        public int roomID;

        public string name="";

        protected bool ready = false;
        public Player() { }
        public Player(Socket socket)
        {
            this.ID = IDCNT++;
            this.onlineFlag = true;
            this.inRoom = false;
            this.socket = socket;
        }

        public void offLine()
        {
            onlineFlag = false;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                
            }catch(Exception e)
            {
            }
            finally
            {
                socket.Close();
            }
            
        }
        public int getID()
        {
            return ID;
        }

        public bool isInRoom()
        {
            return inRoom;
        }

        public int getRoomID()
        {
            return roomID;
        }

        public bool online()
        {
            return onlineFlag;
        }

        public void enterRoom(int roomID)
        {
            inRoom = true;
            this.roomID = roomID;
        }

        public void exitRoom()
        {
            inRoom = false;
            roomID = -1;
        }

        public Socket getSocket()
        {
            return socket;
        }

        public void setName(string s)
        {
            if(name=="")
                name = s;
        }

        public string getName()
        {
            return name;
        }

        public bool getReady()
        {
            return ready;
        }

        public void setReady()
        {
            ready = true;
        }
        public void setNotReady()
        {
            ready = false;
        } 

        public string getUserName()
        {
            return ID + "_" + name;
        }
    }
}
