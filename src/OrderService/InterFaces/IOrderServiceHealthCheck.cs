using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.InterFaces
{
    [ServiceContract]
    public interface IOrderServiceHealthCheck
    {
        [OperationContract]
        string HealthCheck();
    }
}
