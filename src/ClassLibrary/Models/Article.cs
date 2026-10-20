using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class Article
    {
        public int ArticleId { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        public byte[] ImageFile { get; set; }

        public int UserSellerId { get; set; }
    }
}
