using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.WcfModels
{
    [DataContract]
    public class UserDtoStatus
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Status { get; set; }
    }
}
