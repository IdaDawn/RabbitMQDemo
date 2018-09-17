using System;
using System.IO;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GJKConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "47.104.65.81" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //创建RPC队列-队列名rpc_queue，非持久化，非排他，非自动删除
                channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                //Qos 预期大小：0；预取数：1；非全局
                channel.BasicQos(0, 1, false);
                //创建消费者
                var consumer = new EventingBasicConsumer(channel);
                //消费消息：队列名rpc_queue，非自动反馈确认；消费者consumer
                channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");
                //添加监听：异步执行程序
                consumer.Received += (model, ea) =>
                {
                    //消息内容
                    var body = ea.Body;
                    //传来的属性
                    IBasicProperties props = ea.BasicProperties;
                    //RPC反馈的属性-channel创建
                    IBasicProperties replyProps = channel.CreateBasicProperties();
                    //RPC 相关标识
                    replyProps.CorrelationId = props.CorrelationId;

                    byte[] responseBytes = null;
                    try
                    {
                        //消息处理
                        responseBytes = ServerExe(ea.Body);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        responseBytes = Encoding.UTF8.GetBytes("");
                    }
                    finally
                    {
                        //发送消息-服务端反馈给客户端的消息
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        //反馈确认--服务端接收的消息的反馈
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static byte[] ServerExe(byte[] body)
        {
            string response = null;
            var message = Encoding.UTF8.GetString(body);
            int n = int.Parse(message);
            Console.WriteLine(" [.] fib({0})", message);
            response = fib(n).ToString();
            return Encoding.UTF8.GetBytes(response);
        }
        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            return fib(n - 1) + fib(n - 2);
        }
    }
}
