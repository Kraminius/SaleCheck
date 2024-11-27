using System.Text.RegularExpressions;

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

    public Task<List<Page>> Analyze(Page page)
    {
        throw new NotImplementedException();
    }

    public Task<List<Page>> GetSitemapLinks(Page page)
    {
        throw new NotImplementedException();
    }
}