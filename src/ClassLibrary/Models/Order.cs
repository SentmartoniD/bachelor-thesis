using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Comment { get; set; }

        public string Address { get; set; }

        public int Price { get; set; }

        public string DateOfOrder { get; set; }

        public int UserBuyerId { get; set; }

        public string ArticleIdsString
        {
            get => string.Join(",", ArticleIds);
            set => ArticleIds = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToList();
        }

        public ICollection<int> ArticleIds { get; set; }

        public string AmountOfArticlesString
        {
            get => string.Join(",", AmountOfArticles);
            set => AmountOfArticles = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse)
                                     .ToList();
        }

        public ICollection<int> AmountOfArticles { get; set; }
    }
}
