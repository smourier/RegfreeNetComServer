using System;
using System.Runtime.Versioning;
using RegfreeNetComServer.Com;

namespace RegfreeNetComServer
{
    [SupportedOSPlatform("windows")]
    internal class Program
    {
        static void Main()
        {
            using var server = new ComServer();
            server.RegisterClassObject<Server>();
            Console.WriteLine("Waiting for clients... Press [ENTER] to quit.");
            Console.ReadLine();
        }
    }
}
