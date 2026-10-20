using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.WcfModels
{
    [DataContract]
    public class UserDtoRegistration
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string DateOfBirth { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string UserType { get; set; }
        [DataMember]
        public byte[] ImageFile { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
