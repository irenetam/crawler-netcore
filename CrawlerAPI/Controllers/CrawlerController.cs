using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrawlerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlerController : ControllerBase
    {
        private readonly ILogger<CrawlerController> _logger;

        public CrawlerController(ILogger<CrawlerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<ProductDetail>> Get()
        {
            var web1 = new HtmlWeb();
            var list = new List<string> { "https://www.lazada.vn/products/kem-duong-am-kiem-dau-sukin-oil-balancing-mattifying-facial-moisturiser-125ml-i522754249-s1055118998.html?spm=a2o4n.searchlistcategory.list.3.76064483jsEGET&search=1",
            "https://www.lazada.vn/products/dau-dua-lam-dep-sense-plus-100ml-i337690296-s543188419.html?spm=a2o4n.officialstores.1001.dstorelist_2_2.25416780Y2hkdq&acm=20180501004&scm=1007.18128.100341.100200300000000"};
            // get answer in non-blocking way

            List<Task<ProductDetail>> tasks = new List<Task<ProductDetail>>();

            list.ForEach(x =>
            {
                tasks.Add(ProcessCards(x));
            });
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<ProductDetail> ProcessCards(string url)
        {
            // instance or static variable
            HttpClient client = new HttpClient();
            var result = new ProductDetail();
            using (var response = await client.GetAsync(url))
            {
                using (var content = response.Content)
                {
                    // read answer in non-blocking way
                    var data = await content.ReadAsStringAsync();
                    //var document = new HtmlDocument();
                    //document.LoadHtml(data);

                    var lazyContent = new HtmlDocument();
                    lazyContent.LoadHtml(data);
                    HtmlNode title = lazyContent.DocumentNode.SelectSingleNode("//*[@class='pdp-mod-product-badge-title']");
                    HtmlNode score = lazyContent.DocumentNode.SelectSingleNode("//span[@class='score-average']");
                    HtmlNode numberOfRating = lazyContent.DocumentNode.SelectSingleNode("//span[@class='score-max']");
                    //Some work with page....
                    result.Title = title.InnerText;
                    result.Score = score.InnerText != null?Convert.ToInt32(score.InnerText): 0;
                    result.NumberOfRating = numberOfRating.InnerText != null ? Convert.ToInt32(numberOfRating.InnerText) : 0;
                }
            }
            return result;

        }
    }
}
