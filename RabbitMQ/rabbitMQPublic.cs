using RabbitMQ.Client;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RabbitMQ
{
    public class rabbitMQPublic
    {
        public static class simplePublisher
        {
            private const string EXCHANGE_NAME = "exchange_demo";
            private const string ROUTING_KEY = "routingkey_demo";
            private const string QUEUE_NAME = "queue_demo";
            private const string IP_ADDRESS = "47.104.65.81";
            private const int PORT = 5672;//RabbitMQ 服务端默认端口5672；
            private const string USER_NAME = "guest";
            private const string PASSWORD = "guest";

            public static void Publicer()
            {
                IConnection con = null;
                IModel channel = null;
                try
                {
                    //01.创建factory
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.HostName = IP_ADDRESS;
                    factory.Port = PORT;
                    factory.UserName = USER_NAME;
                    factory.Password = PASSWORD;
                    //02.创建Connection
                    con = factory.CreateConnection();
                    //03.创建Channel
                    channel = con.CreateModel();
                    //创建一个type = "direct" 、持久化的、非自动删除的交换器
                    channel.ExchangeDeclare(EXCHANGE_NAME, "direct", true, false, null);
                    //创建一个持久的、非排他的、非自动删除的队列
                    channel.QueueDeclare(QUEUE_NAME, true, false, false, null);
                    //将交换器与队列通过路由键绑定            
                    channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);//channel.ExchangeBind()
                                                                                    //04.创建消息并发送
                    string message = "Hello Word!";
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.BasicPublish(EXCHANGE_NAME, ROUTING_KEY, properties, body);

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


        }

    }
}
