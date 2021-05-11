using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlerAPI
{
    public class ProductDetail
    {

        public decimal? Rating { get; set; }
        public decimal? RateCount { get; set; }
        public decimal? ReviewCount { get; set; }
        public int[] Scores { get; set; }
        public string Url { get; set; }
        public bool? IsMall { get; set; }
    }
}
