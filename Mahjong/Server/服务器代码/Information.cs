using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mahjong_server
{
    interface TypeToInt
    {
        public int typeToInt();
    }
    class Information : AutoSerializable<Information>, TypeToInt
    {
        public string a { get; set; }
        public string s { get; set; }
        public Information(int a, string s = "123")
        {
            this.a = a+"";
            this.s = s;
        }

        public Information(string a,string s)
        {
            this.a = a;
            this.s = s;
        }

        public void print()
        {
            Console.WriteLine("out");
        }
        public Information convert(Object obj)
        {
            return (Information)obj;
        }

        public Information(String s)
        {
            Information t = JsonSerializer.Deserialize<Information>(s);
            this.a = t.a;
            this.s = t.s;
        }

        public Information Deserialize(String s)
        {
            Information t = JsonSerializer.Deserialize<Information>(s);
            return t;
        }

        public Information() { }

        public int typeToInt()
        {
            return 1;
        }
    }
}
