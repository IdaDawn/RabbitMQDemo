using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RabbitMQ
{
    public static class SimpleConsum
    {
        private const string QUEUE_NAME = "queue_demo";
        private const string IP_ADDRESS = "47.104.65.81";
        private const int PORT = 5672;//RabbitMQ 服务端默认端口5672；
        private const string USER_NAME = "guest";
        private const string PASSWORD = "guest";


        public static void Consumer()
        {
            IConnection con = null;
            IModel channel = null;
            try
            {
                //01.创建factory
                ConnectionFactory factory = new ConnectionFactory();
                factory.UserName = USER_NAME;
                factory.Password = PASSWORD;
                //02.创建连接
                con = factory.CreateConnection();
                //03.创建channel
                channel = con.CreateModel();
                //创建一个持久的、非排他的、非自动删除的队列
                channel.QueueDeclare(QUEUE_NAME, true, false, false, null);
                //队列最大接收未被ack的消息的个数
                channel.BasicQos(64, 1000, true);
                //04.创建消费者-监听方式
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    Run(body);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(QUEUE_NAME, false, consumer);
            }
            catch (IOException ioE)
            {
                throw;
            }
            catch (SocketException socketEx)//RabbitMQ 用TCP协议，这里除了socket异常
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //05.关闭资源
                if (channel != null)
                    channel.Close();
                if (con != null)
                    con.Close();
            }

        }

        private static void Run(byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        }

    }

}
