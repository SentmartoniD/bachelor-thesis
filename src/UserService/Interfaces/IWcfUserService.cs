using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;

namespace UserService.Interfaces
{
    [ServiceContract]
    public interface IWcfUserService
    {
        [OperationContract]
        UserDtoRegistration AddUser(UserDtoRegistration newUser);
        [OperationContract]
        string LoginUser(UserDtoLogin userDto);
        [OperationContract]
        UserDtoRegistration GetUserById(int id);
        [OperationContract]
        UserDtoApprovedVerified GetUserByEmail(string email);
        [OperationContract]
        List<UserDtoApprovedVerified> GetUsers();
        [OperationContract]
        UserDtoRegistration UpdateUser(int id, UserDtoRegistration userDtoRegistration);
        [OperationContract]
        UserDtoStatus UpdateUserStatus(UserDtoStatus userDtoStatus);
        [OperationContract]
        bool UploadImageMS(MemoryStream ms, string email);
    }
}
