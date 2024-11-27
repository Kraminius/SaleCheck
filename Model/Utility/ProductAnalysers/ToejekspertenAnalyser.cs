using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SaleCheck.Model.Utility.ProductAnalysers;

public class ToejekspertenAnalyser : IProductAnalyser
{
    public async Task<List<Page>> GetRobotsSitemapLink(Page page)
    {
        string html = await page.GetHtmlContent();
        
        if (html == null) return new List<Page>();

        string pattern = @"Sitemap:\s*(https?://\S+)";
        MatchCollection matches = Regex.Matches(html, pattern);
        List<Page> sitemapPages = new List<Page>();

        foreach (Match match in matches)
        {
            string link = match.Groups[1].Value;

            if (!link.StartsWith("http"))
            {
                link = page.GetUrl().TrimEnd('/') + link;
            }

            if (!link.Contains(".dk") && !link.Contains("da_dk"))
            {
                continue;
            }

            sitemapPages.Add(new Page(SampleSites.ToejEksperten.Title, link));

        }

        return sitemapPages; 
        
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
            string link = match.Value.Trim('>', '<');
            pages.Add(new Page(SampleSites.VinduesGrossisten.Title,link));
        }

        return pages;
        
    }
    
    public async Task<List<ProductItem>> Analyze(Page page)
    {
        List<ProductItem> products = new List<ProductItem>();
        var htmlDoc = new HtmlDocument();
        string? content = await page.GetHtmlContent();
        if (content == null) return new List<ProductItem>();
        htmlDoc.LoadHtml(content);
        
        var productNodes = htmlDoc.DocumentNode.SelectNodes(
            "//a[contains(@class, 'ela0j04e0') and contains(@class, 'css-12clykb') and contains(@class, 'e127r8ub0')]");

        if (productNodes == null)
        {
            return products;
        }

        foreach (var productNode in productNodes)
        {
            
        }
        
        
        
        throw new NotImplementedException();
    }

    
}