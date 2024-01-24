using System;
using System.Runtime.InteropServices;

namespace RegfreeNetComServer
{
    [Guid("AF080472-F173-4D9D-8BE7-435776617347")]
    public class Server : IServer
    {
        public double ComputePi()
        {
            Console.WriteLine("compute!");
            return Math.PI;
        }
    }
}
