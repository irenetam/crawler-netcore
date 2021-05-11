using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            var list = new List<string> {
                "https://www.lazada.vn/products/kem-sieu-duong-trang-da-mat-white-face-thai-lan-dc-i496126055-s966574375.html?&search=pdp_v2v?spm=a2o4n.pdp_revamp.recommendation_2.2.5db266ecPYZHrY&mp=1&scm=1007.16389.126158.0&clickTrackInfo=47545e91-9e08-4ca1-8c8b-de2ab3dfd13b__496126055__2283__trigger2i__202835__0.148__0.148__0.0__0.0__0.0__0.148__1__8__PDPV2V__251__null__1515062905%201498575426%20830083863__0____19000.0__0.0__4.713804713804714__891__19000.0__78027,88271,88708,89297,89342,89401,89487,89491,89527,90649,91254,92232,92271,92281,92853,92928,93870,93939,94227,95266,103190,110662,111134,115409,115466,115470__null__null__null__3650.16544_3650.16536_955.3632__null__13499__null__0.0__0.0__starplus.2__",
                "https://www.lazada.vn/products/dau-dua-lam-dep-sense-plus-100ml-i337690296-s543188419.html?spm=a2o4n.officialstores.1001.dstorelist_2_2.25416780Y2hkdq&acm=20180501004&scm=1007.18128.100341.100200300000000",
                "https://www.lazada.vn/products/tinh-chat-lam-trang-hong-chuyen-nghiep-estee-lauder-perfectionist-pro-rapid-brightening-treatment-with-ferment-vitamin-c-i510786999-s1014142430.html?spm=a2o4n.searchlistcategory.list.4.50916e33t6BGn9&search=1&freeshipping=1",
                "https://www.lazada.vn/products/serum-thay-da-khong-sung-nhan-sam-mq-skin-phien-ban-cao-cap-gingseng-repair-serum-premium-80ml-i1219981317-s4520097161.html?spm=a2o4n.searchlistcategory.list.13.50916e33ypwXkn&search=1",
                "https://www.lazada.vn/products/bo-kem-huas-huli-co-tien-do-ngay-va-dem-danh-cho-da-nam-tan-nhang-i1091350487-s3788788152.html?spm=a2o4n.searchlistcategory.list.21.50916e33TGce5X&search=1",
                "https://www.lazada.vn/products/kem-nen-che-khuyet-diem-maycheer-cover-face-hang-noi-dia-trung-i1085860246-s3760780043.html?&search=pdp_same_topselling?spm=a2o4n.pdp_revamp.recommendation_1.3.5db266ecMJ0Wzm&mp=1&scm=1007.16389.126158.0&clickTrackInfo=bdf3233f-320e-4dda-a372-3b6e582542ea__1085860246__6999__trigger2i__224806__0.05__0.05__0.0__0.0__0.0__0.05__2__null__null__null__null__null__null____25000.0__0.0__5.0__3__25000.0____null__null__null__3650.16539_3650.16544_955.3632__null__13426__null__0.0__0.0______",
                "https://www.lazada.vn/products/nuoc-hoa-desire-100ml-i1291269692-s4907363309.html?spm=a2o4n.searchlistcategory.list.11.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/hang-cao-cap-nuoc-hoa-nam-boss-cao-cap-100ml-mui-huong-sang-trong-manh-me-loi-cuon-thanh-lich-bao-hanh-2-nam-i1290790863-s4900190398.html?spm=a2o4n.searchlistcategory.list.13.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/voucher-20kfreeshipcombo-2-nuoc-hoa-toan-than-cao-cap-enchanteur-huong-princess-100mlchai-i1043122989-s3535896301.html?spm=a2o4n.searchlistcategory.list.15.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/nuoc-hoa-perfumersgift-midsummer-firefly-miniso-25ml-nuoc-hoa-thom-lau-nu-nuoc-hoa-nu-thom-lau-nuoc-hoa-nu-dau-thom-nu-dau-thom-nu-i1224132618-s4535337715.html?spm=a2o4n.searchlistcategory.list.17.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/xit-thom-body-rseries-body-spray-75ml-i296602957-s473956163.html?spm=a2o4n.searchlistcategory.list.21.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/nuoc-hoa-edt-x-men-for-boss-intense-mui-huong-tram-day-noi-luc-49ml-i204631671-s254952397.html?spm=a2o4n.searchlistcategory.list.23.2f91139e4DrrQp&search=1",
                "https://www.lazada.vn/products/serum-giam-mun-trang-da-truesky-premium-chiet-xuat-tram-tra-thien-nhien-20ml-whitening-acne-serum-i610542624-s1403724996.html?spm=a2o4n.searchlistcategory.list.4.189e6e337sQnak&search=1",
                "https://www.lazada.vn/products/tinh-chat-duong-am-tai-tao-va-phuc-hoi-da-96-oc-sen-cosrx-advanced-snail-96-mucin-power-essence-100ml-i364218556-s603380479.html?spm=a2o4n.searchlistcategory.list.48.189e6e337sQnak&search=1",
                "https://www.lazada.vn/products/kem-duong-trang-min-va-giam-tham-nam-ban-ngay-loreal-paris-white-perfect-clinical-day-spf-19pa-50ml-kem-ngay-i258034260-s357408006.html?spm=a2o4n.searchlistcategory.list.44.189e6e337sQnak&search=1",
                "https://www.lazada.vn/products/tinh-chat-hong-7-loai-vitamin-b-phuc-hoi-va-lam-sang-da-cnp-laboratory-vita-b-energy-ampule-15ml-i343772004-s556812780.html?spm=a2o4n.searchlistcategory.list.68.189e6e337sQnak&search=1",
                "https://www.lazada.vn/products/bo-duong-trang-klairs-serum-vitamin-c-va-kem-duong-2-in1-freshly-juiced-i867290587-s2467756196.html?spm=a2o4n.searchlistcategory.list.76.189e6e337sQnak&search=1",
                "https://tiki.vn/son-duong-dior-addict-lip-glow-004-coral-3-5g-p72186695.html?spid=82956493",
                "https://tiki.vn/son-duong-dior-addict-lip-glow-004-coral-3-5g-p72186695.html?spid=82956493",
                "https://www.sendo.vn/khau-trang-y-te-4-lop-cao-cap-loai-1-khang-khuan-50-caihop-chauducmedi-chuan-iso-41907426.html?fromItem=68012048&source_block_id=flash-sale&source_page_id=flash-sale&source_position=2&source_slot_id=50214129&source_category_group_id=0&source_info=desktop2_60_1620696832645_4620b0f4-14fa-4189-bab2-d7b28b47687b__default__2__",
                "https://www.sendo.vn/ban-de-may-tinh-ban-lam-viec-20168135.html?source_block_id=listing_products&source_info=desktop2_60_1620697616216_4620b0f4-14fa-4189-bab2-d7b28b47687b_5_algo13_0_9_7_-1&source_page_id=cate3_listing_v2_desc"
            };
            // get answer in non-blocking way

            return await CrawlData(list);
        }

        private async Task<List<ProductDetail>> CrawlData(List<string> list)
        {
            List<Task<ProductDetail>> tasks = new List<Task<ProductDetail>>();

            list.ForEach(url =>
            {
                tasks.Add(ProcessCrawls(url));
            });
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<ProductDetail> ProcessCrawls(string url)
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
                    string script = null;
                    ProductType productType = GetProductType(url);

                    switch (productType)
                    {
                        case ProductType.Lazada:
                            script = lazyContent.DocumentNode.Descendants()
                                     .Where(n => n.Name == "script")
                                     .Select(x => x.InnerText)?.Where(x => x.Contains("average")).FirstOrDefault();
                            result.IsMall = lazyContent.DocumentNode.SelectSingleNode("//*[@class='pdp-mod-product-badge']") != null;
                            
                            if(!string.IsNullOrEmpty(script))
                            {
                                result = ConvertLazadaData(script, url);
                            }
                            break;
                        case ProductType.Tiki:
                            script = lazyContent.DocumentNode.Descendants()
                                     .Where(n => n.Name == "script")
                                     .Select(x => x.InnerText)?.Where(x => x.Contains("AggregateRating")).FirstOrDefault();
                            if (!string.IsNullOrEmpty(script))
                            {
                                result = ConvertTikiData(script, url);
                            }
                            break;
                        case ProductType.Sendo:
                            script = lazyContent.DocumentNode.Descendants()
                                     .Where(n => n.Name == "script")
                                     .Select(x => x.InnerText)?.Where(x => x.Contains("AggregateRating")).FirstOrDefault();
                            if (!string.IsNullOrEmpty(script))
                            {
                                result = ConvertSendoData(script, url);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return result;

        }

        private ProductDetail ConvertSendoData(string script, string url)
        {
            var result = new ProductDetail();
            var ratingsObject = JObject.Parse(script);
            result.Url = url;
            result.Rating = ((JProperty)ratingsObject.Last).Value["ratingCount"].ToObject<decimal?>();
            result.RateCount = ((JProperty)ratingsObject.Last).Value["ratingCount"].ToObject<decimal?>();
            return result;
        }

        private ProductDetail ConvertTikiData(string script, string url)
        {
            var result = new ProductDetail();
            var ratingsObject = JObject.Parse(script);
            result.Url = url;
            result.Rating = ratingsObject["ratingValue"].ToObject<decimal?>();
            result.RateCount = ratingsObject["ratingCount"].ToObject<decimal?>();
            return result;
        }
        private ProductDetail ConvertLazadaData(string script, string url)
        {
            var result = new ProductDetail();
            var fromIdex = script.IndexOf("average");
            var toIndex = script.IndexOf("reportReasons");
            var ratings = script.Substring((fromIdex - 2), (toIndex - fromIdex));
            var ratingsObject = JObject.Parse(ratings);
            result.Url = url;
            result.Rating = ratingsObject["average"].ToObject<decimal?>();
            result.RateCount = ratingsObject["rateCount"].ToObject<decimal?>();
            result.ReviewCount = ratingsObject["reviewCount"].ToObject<decimal?>();
            result.Scores = ratingsObject["scores"].ToObject<List<int>>().ToArray();
            return result;
        }

        private ProductType GetProductType(string url)
        {
            ProductType productType = ProductType.Tiki;
            if (url.Contains("lazada"))
            {
                productType = ProductType.Lazada;
            }
            else if (url.Contains("shopee"))
            {
                productType = ProductType.Shoppee;
            } else if (url.Contains("sendo"))
            {
                productType = ProductType.Sendo;
            }
            return productType;
        }
    }
    enum ProductType
    {
        Lazada,
        Tiki,
        Shoppee,
        Sendo
    }
}
