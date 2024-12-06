
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Amazon.SecurityToken.Model;
using HtmlAgilityPack;
using SaleCheck.HtmlLib;
using Tag = SaleCheck.HtmlLib.Tag;

namespace SaleCheck.Model.Utility.ProductAnalysers 
{
    public class SportyFitAnalyser : IProductAnalyser
    {
        public async Task<List<Page>> GetRobotsSitemapLink(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new List<Page>();

            string pattern = @"Sitemap:\s*(https?://\S+|/\S+)";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<Page> pages = new List<Page>();
            Console.WriteLine("Found {0} sitemap entries in robots.txt.", matches.Count);

            foreach (Match match in matches)
            {
                string sitemapUrl = match.Groups[1].Value;

                // Ensure full URL for relative links
                if (!sitemapUrl.StartsWith("http"))
                {
                    sitemapUrl = page.GetUrl().TrimEnd('/') + sitemapUrl;
                }

                // Filter only Danish links or links in Danish context
                if (!sitemapUrl.Contains(".dk") && !sitemapUrl.Contains("da_dk"))
                {
                    continue;
                }

                var sitemapPages = await GetAllPagesFromSitemap(sitemapUrl, SampleSites.SportyFit.Title);
                pages.AddRange(sitemapPages);
            }

            return pages;
        }

        public async Task<List<Page>> GetSitemapLinks(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new List<Page>();
            string pattern = @">https://[^\s<]+<";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<Page> pages = new List<Page>();

            foreach (Match match in matches)
            {
                // Remove the wrapping '>' and '<'
                string link = match.Value.Trim('>', '<');
                pages.Add(new Page(SampleSites.SportyFit.Title,link));
            }

            return pages;
        }
        
        private static decimal ParsePrice(string? priceText)
        {
            if(priceText == null) return -1;
            string result = Regex.Replace(priceText, @"[^0-9,\.]", ""); //Remove unnecessary characters
            while(result[0] == ',' || result[0] == '.') result = result.Remove(0, 1);
            while(result[^1] == ',' || result[^1] == '.') result = result.Remove(result.Length-1, 1);
            return decimal.Parse(result);
        }
        public async Task<List<ProductItem>> Analyze(Page page)
        {
            string? content = await page.GetHtmlContent();
            
            if (content == null) return new List<ProductItem>();
            Html html = new Html(content);
            List<Tag> parents = html.SearchForTags(new Filter().Tag("div").Property("class", "e-loop-item"));
            List<ProductItem> productItems = new List<ProductItem>();
            foreach (Tag parent in parents)
            {
                
                string? url = html.SearchForTag(new Filter().Tag("a").Property("class", "elementor-element").Property("data-element_type", "container"), parent)?.GetProperty("href");
                string? name = html.SearchForTag(new Filter().Tag("p").Property("class", "elementor-heading-title"), parent)?.ChildTags[0].Content?.Trim();
                string? id = name;
                //<p class="price product-page-price price-on-sale price-not-in-stock">
                 
                
                decimal price = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Original price"), parent)?.Content?.Trim());
                decimal discount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Current price"), parent)?.Content?.Trim());
                decimal noDiscount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "woocommerce-Price-amount"), parent)?.ChildTags[0]?.Content?.Trim());
                
                if (id == null || url == null || name == null) continue;
                if (price == -1 || discount == -1)
                {
                    productItems.Add(new ProductItem(url, name, id, noDiscount));
                }
                else if(discount == -1) productItems.Add(new ProductItem(url, name, id, price));
                else productItems.Add(new ProductItem(url, name, id, price, discount));
            }
            return productItems;
        }
        
        public async Task<List<string>> GetAllUrlsFromSitemap(string sitemapUrl)
        {
            var allUrls = new List<string>();
            await ProcessSitemap(sitemapUrl, allUrls);
            return allUrls;
        }

        private async Task ProcessSitemap(string sitemapUrl, List<string> allUrls)
        {
            Console.WriteLine($"Processing sitemap: {sitemapUrl}");
            string content = await GetContent(sitemapUrl);

            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"Failed to retrieve content from {sitemapUrl}");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing XML from {sitemapUrl}: {ex.Message}");
                return;
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns", "http://www.sitemaps.org/schemas/sitemap/0.9");

            // Check if it's a sitemap index
            XmlNodeList sitemapNodes = xmlDoc.SelectNodes("//ns:sitemap/ns:loc", nsmgr);
            if (sitemapNodes != null && sitemapNodes.Count > 0)
            {
                // It's a sitemap index; process each nested sitemap
                foreach (XmlNode sitemapNode in sitemapNodes)
                {
                    string nestedSitemapUrl = sitemapNode.InnerText.Trim();
                    await ProcessSitemap(nestedSitemapUrl, allUrls);
                }
            }
            else
            {
                // It's a URL sitemap; extract URLs
                XmlNodeList urlNodes = xmlDoc.SelectNodes("//ns:url/ns:loc", nsmgr);
                foreach (XmlNode urlNode in urlNodes)
                {
                    string url = urlNode.InnerText.Trim();
                    // Filter only Danish links or links in Danish context
                    if (url.Contains(".dk") || url.Contains("da_dk"))
                    {
                        allUrls.Add(url);
                        Console.WriteLine($"Added URL: {url}");
                    }
                }
            }
        }

        private async Task<string> GetContent(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching content from {url}: {ex.Message}");
                    return null;
                }
            }
        }
        
        private async Task<List<Page>> GetAllPagesFromSitemap(string sitemapUrl, string websiteTitle)
        {
            var urls = await GetAllUrlsFromSitemap(sitemapUrl);
            List<Page> pages = urls.Select(url => new Page(websiteTitle, url)).ToList();
            return pages;
        }

        
    }
    
    
}
