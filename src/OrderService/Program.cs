using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using OrderService.Providers;
using OrderService.InterFaces;

namespace OrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost wcfOrderService = new ServiceHost(typeof(OrderServiceProvider));
            wcfOrderService.AddServiceEndpoint(typeof(IWcfOrderService), new NetTcpBinding(), new Uri("net.tcp://localhost:4047/IWcfOrderService"));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            wcfOrderService.Description.Behaviors.Add(smb);
            Binding mexbinding = MetadataExchangeBindings.CreateMexTcpBinding();
            wcfOrderService.AddServiceEndpoint(typeof(IMetadataExchange), mexbinding, "net.tcp://localhost:4047/IWcfOrderService/mex");
            wcfOrderService.Open();

            ServiceHost wcfOrderServiceHealthCheck = new ServiceHost(typeof(OrderServiceHealthCheckProvider));
            wcfOrderServiceHealthCheck.AddServiceEndpoint(typeof(IOrderServiceHealthCheck), new NetTcpBinding(), new Uri("net.tcp://localhost:5002/IOrderServiceHealthCheck"));
            wcfOrderServiceHealthCheck.Open();

            Console.WriteLine("Pritisnite [Enter] za zaustavljanje servisa.");
            Console.ReadKey();

            wcfOrderService.Close();
            wcfOrderServiceHealthCheck.Close();
        }
    }
}
