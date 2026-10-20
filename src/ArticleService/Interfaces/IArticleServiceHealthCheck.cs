using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ArticleService.Interfaces
{
    [ServiceContract]
    public interface IArticleServiceHealthCheck
    {
        [OperationContract]
        string HealthCheck();
    }
}
