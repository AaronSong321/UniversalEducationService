using HIT.UES.Server.Service;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RestServerTest();
            server.BootServer();
            Console.ReadLine();
        }
    }
}
