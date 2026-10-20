using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.WcfModels
{
    [DataContract]
    public class WcfOrder
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
        public int UserBuyerId { get; set; }
        [DataMember]
        public string ArticleIdsString
        {
            get => string.Join(",", ArticleIds);
            set => ArticleIds = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToList();
        }
        [DataMember]
        public ICollection<int> ArticleIds { get; set; }
        [DataMember]
        public string AmountOfArticlesString
        {
            get => string.Join(",", AmountOfArticles);
            set => AmountOfArticles = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToList();
        }
        [DataMember]
        public ICollection<int> AmountOfArticles { get; set; }
    }
}
