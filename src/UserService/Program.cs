using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UserService.Providers;
using UserService.Interfaces;
using System.ServiceModel.Description;
using ClassLibrary.WcfModels;
using System.ServiceModel.Channels;

namespace UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ServiceHost wcfUserService = new ServiceHost(typeof(UserServiceProvider));
            wcfUserService.AddServiceEndpoint(typeof(IWcfUserService), new NetTcpBinding(),
                new Uri("net.tcp://localhost:4005/IWcfUserService"));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            wcfUserService.Description.Behaviors.Add(smb);
            Binding mexbinding = MetadataExchangeBindings.CreateMexTcpBinding();
            wcfUserService.AddServiceEndpoint(typeof(IMetadataExchange), mexbinding, 
                "net.tcp://localhost:4005/IWcfUserService/mex");
            wcfUserService.Open();

            ServiceHost wcfInternalUserService = new ServiceHost(typeof(InternalUserServiceProvider));
            wcfInternalUserService.AddServiceEndpoint(typeof(IWcfInternalUserService),
                new NetTcpBinding(), new Uri("net.tcp://localhost:4000/IWcfInternalUserService"));
            wcfInternalUserService.Open();


            ServiceHost wcfUserServiceHealthCheck = new ServiceHost(typeof(UserServiceHealthCheckProvider));
            wcfUserServiceHealthCheck.AddServiceEndpoint(typeof(IUserServiceHealthCheck), 
                new NetTcpBinding(), new Uri("net.tcp://localhost:5003/IUserServiceHealthCheck"));
            wcfUserServiceHealthCheck.Open();


            Console.WriteLine("Pritisnite [Enter] za zaustavljanje servisa.");
            Console.ReadKey();
            
            wcfUserService.Close();
            wcfInternalUserService.Close();
            wcfUserServiceHealthCheck.Close();
        }
    }
}
