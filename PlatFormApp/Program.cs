using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;

namespace PlatFormApp
{
    class Program
    {
      
        static void Main(string[] args)
        {
            RabbitMQ.RpcClient client = new RabbitMQ.RpcClient();
            client.Call("Hello World!");
            Console.WriteLine("Hello World!");
        }
    }
}
