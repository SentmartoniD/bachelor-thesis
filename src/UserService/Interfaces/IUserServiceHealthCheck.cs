using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
namespace UserService.Interfaces
{
    [ServiceContract]
    public interface IUserServiceHealthCheck
    {
        [OperationContract] 
        string HealthCheck();
    }
}
