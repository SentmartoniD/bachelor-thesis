using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.WcfModels
{
    [DataContract]
    public class ArticleDto
    {
        [DataMember]
        public int ArticleId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public byte[] ImageFile { get; set; }
        [DataMember]
        public WcfUser UserSeller { get; set; }
    }
}
