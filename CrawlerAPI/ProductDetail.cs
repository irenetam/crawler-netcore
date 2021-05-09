using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlerAPI
{
    public class ProductDetail
    {

        public decimal? average { get; set; }
        public decimal? rateCount { get; set; }
        public decimal? reviewCount { get; set; }
        public int[] scores { get; set; }
    }
}
