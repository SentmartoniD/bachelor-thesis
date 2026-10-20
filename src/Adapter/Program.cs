using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using ArticleService.Interfaces;
using OrderService.InterFaces;
using System.ServiceModel;
using System.Threading;

namespace Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IUserServiceHealthCheck> userServiceChannelFactory = 
                new ChannelFactory<IUserServiceHealthCheck>(new NetTcpBinding(), 
                new EndpointAddress("net.tcp://localhost:5003/IUserServiceHealthCheck"));
            ChannelFactory<IArticleServiceHealthCheck> articleServiceChannelFactory = 
                new ChannelFactory<IArticleServiceHealthCheck>(new NetTcpBinding(), 
                new EndpointAddress("net.tcp://localhost:5001/IArticleServiceHealthCheck"));
            ChannelFactory<IOrderServiceHealthCheck> orderServiceChannelFactory =
                new ChannelFactory<IOrderServiceHealthCheck>(new NetTcpBinding(),
                new EndpointAddress("net.tcp://localhost:5002/IOrderServiceHealthCheck"));
            while (true) {
                try
                {
                    IUserServiceHealthCheck userServiceHealthCheckChannel = userServiceChannelFactory.CreateChannel();
                    Console.WriteLine(userServiceHealthCheckChannel.HealthCheck());
                }
                catch (Exception e) {
                    Console.WriteLine("Status : ERROR, UserService is unreachable!");
                }
                try
                {
                    IArticleServiceHealthCheck articleServiceHealthCheckChannel = articleServiceChannelFactory.CreateChannel();
                    Console.WriteLine(articleServiceHealthCheckChannel.HealthCheck());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Status : ERROR, ArticleService is unreachable!");
                }
                try
                {
                    IOrderServiceHealthCheck orderServiceHealthCheckChannel = orderServiceChannelFactory.CreateChannel();
                    Console.WriteLine(orderServiceHealthCheckChannel.HealthCheck());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Status : ERROR, OrderService is unreachable!");
                }
                Console.WriteLine("------------------------------------------------------------------");
                Thread.Sleep(3000);
            }
        }
    }
}
