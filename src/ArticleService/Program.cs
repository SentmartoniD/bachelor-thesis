using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ArticleService.Providers;
using ArticleService.Interfaces;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace ArticleService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost wcfUserService = new ServiceHost(typeof(ArticleServiceProvider));
            wcfUserService.AddServiceEndpoint(typeof(IWcfArticeService), new NetTcpBinding(), new Uri("net.tcp://localhost:4007/IWcfArticeService"));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            wcfUserService.Description.Behaviors.Add(smb);
            Binding mexbinding = MetadataExchangeBindings.CreateMexTcpBinding();
            wcfUserService.AddServiceEndpoint(typeof(IMetadataExchange), mexbinding, "net.tcp://localhost:4007/IWcfArticeService/mex");
            wcfUserService.Open();
            

            ServiceHost wcfInternalArticleService = new ServiceHost(typeof(InternalArticleServiceProvider));
            wcfInternalArticleService.AddServiceEndpoint(typeof(IWcfInternalArticleService), new NetTcpBinding(), new Uri("net.tcp://localhost:4011/IWcfInternalArticleService"));
            wcfInternalArticleService.Open();

            ServiceHost wcfArticleServiceHealthCheck = new ServiceHost(typeof(ArticleServiceHealthCheckProvider));
            wcfArticleServiceHealthCheck.AddServiceEndpoint(typeof(IArticleServiceHealthCheck), new NetTcpBinding(), new Uri("net.tcp://localhost:5001/IArticleServiceHealthCheck"));
            wcfArticleServiceHealthCheck.Open();

            Console.WriteLine("Pritisnite [Enter] za zaustavljanje servisa.");
            Console.ReadKey();

            wcfUserService.Close();
            wcfInternalArticleService.Close();
            wcfArticleServiceHealthCheck.Close();
        }
    }
}
