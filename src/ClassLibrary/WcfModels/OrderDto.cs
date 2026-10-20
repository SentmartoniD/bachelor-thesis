using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.WcfModels
{
    [DataContract]
    public class OrderDto
    {
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public string DateOfOrder { get; set; }
        [DataMember]
        public ICollection<int> ArticleIds { get; set; }
        [DataMember]
        public ICollection<int> AmountOfArticles { get; set; }
    }
}
