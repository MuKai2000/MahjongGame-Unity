using System;
using System.Collections.Generic;
using System.Text;

namespace Mahjong_server
{
    class Message
    {
        public Type type;
        public Object obj;
        public int ID;

        public Message() { }
        public Message(Type type,Object obj,int ID)
        {
            this.type = type;
            this.obj = obj;
            this.ID = ID;
        }
    }
}
