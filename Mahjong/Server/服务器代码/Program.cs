using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mahjong_server
{
    class Program
    {
        static void Main(string[] args)
        {

            /*
            Test a = new Test(3, "hhh"), b = new Test(43);
            string s = JsonSerializer.Serialize(a);
            Type type = a.GetType();
            Object obj = (Object)Activator.CreateInstance(type, s);
            // Object obj = (Object)(type.GetMethod("deserialize").Invoke(new object(), new object[] { s }));
            b = (Test)obj;
            */
            
            Console.WriteLine("Hello World");
            LobbyServer lobby = LobbyServer.getInstance();
        }
    }
}
