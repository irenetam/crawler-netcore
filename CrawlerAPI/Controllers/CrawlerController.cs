using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jurassic.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var list = new List<string> { "https://www.lazada.vn/products/kem-sieu-duong-trang-da-mat-white-face-thai-lan-dc-i496126055-s966574375.html?&search=pdp_v2v?spm=a2o4n.pdp_revamp.recommendation_2.2.5db266ecPYZHrY&mp=1&scm=1007.16389.126158.0&clickTrackInfo=47545e91-9e08-4ca1-8c8b-de2ab3dfd13b__496126055__2283__trigger2i__202835__0.148__0.148__0.0__0.0__0.0__0.148__1__8__PDPV2V__251__null__1515062905%201498575426%20830083863__0____19000.0__0.0__4.713804713804714__891__19000.0__78027,88271,88708,89297,89342,89401,89487,89491,89527,90649,91254,92232,92271,92281,92853,92928,93870,93939,94227,95266,103190,110662,111134,115409,115466,115470__null__null__null__3650.16544_3650.16536_955.3632__null__13499__null__0.0__0.0__starplus.2__",
            "https://www.lazada.vn/products/dau-dua-lam-dep-sense-plus-100ml-i337690296-s543188419.html?spm=a2o4n.officialstores.1001.dstorelist_2_2.25416780Y2hkdq&acm=20180501004&scm=1007.18128.100341.100200300000000",
            "https://www.lazada.vn/products/tinh-chat-lam-trang-hong-chuyen-nghiep-estee-lauder-perfectionist-pro-rapid-brightening-treatment-with-ferment-vitamin-c-i510786999-s1014142430.html?spm=a2o4n.searchlistcategory.list.4.50916e33t6BGn9&search=1&freeshipping=1",
            "https://www.lazada.vn/products/serum-thay-da-khong-sung-nhan-sam-mq-skin-phien-ban-cao-cap-gingseng-repair-serum-premium-80ml-i1219981317-s4520097161.html?spm=a2o4n.searchlistcategory.list.13.50916e33ypwXkn&search=1",
            "https://www.lazada.vn/products/bo-kem-huas-huli-co-tien-do-ngay-va-dem-danh-cho-da-nam-tan-nhang-i1091350487-s3788788152.html?spm=a2o4n.searchlistcategory.list.21.50916e33TGce5X&search=1",
            "https://www.lazada.vn/products/kem-nen-che-khuyet-diem-maycheer-cover-face-hang-noi-dia-trung-i1085860246-s3760780043.html?&search=pdp_same_topselling?spm=a2o4n.pdp_revamp.recommendation_1.3.5db266ecMJ0Wzm&mp=1&scm=1007.16389.126158.0&clickTrackInfo=bdf3233f-320e-4dda-a372-3b6e582542ea__1085860246__6999__trigger2i__224806__0.05__0.05__0.0__0.0__0.0__0.05__2__null__null__null__null__null__null____25000.0__0.0__5.0__3__25000.0____null__null__null__3650.16539_3650.16544_955.3632__null__13426__null__0.0__0.0______"};
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
                    var lazyContent = new HtmlDocument();
                    lazyContent.LoadHtml(data);
                    Thread.Sleep(1000);
                    var scripts = lazyContent.DocumentNode.Descendants()
                             .Where(n => n.Name == "script")
                             .Select(x => x.InnerText)?.Where(x => x.Contains("average")).ToArray();
                    foreach(var item in scripts)
                    {
                        result = ConvertRating(item);
                    }
                }
            }
            return result;

        }

        private ProductDetail ConvertRating(string item)
        {
            var result = new ProductDetail();
            var fromIdex = item.IndexOf("average");
            var toIndex = item.IndexOf("reportReasons");
            var ratings = item.Substring((fromIdex - 2), (toIndex - fromIdex));
            var ratingsObject = JObject.Parse(ratings);
            result.average = ratingsObject["average"].ToObject<decimal?>();
            result.rateCount = ratingsObject["rateCount"].ToObject<decimal?>();
            result.reviewCount = ratingsObject["reviewCount"].ToObject<decimal?>();
            result.scores = ratingsObject["scores"].ToObject<List<int>>().ToArray();
            return result;
        }
    }
}
