using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;

namespace UserService.Interfaces
{
    [ServiceContract]
    public interface IWcfInternalUserService
    {
        [OperationContract]
        WcfUser GetUserByEmail(string email);
    }
}
