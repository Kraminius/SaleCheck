namespace SaleCheck.Model.Utility;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SiteMap(string url)
{
    private readonly Page _siteMap = new Page(url);
    private readonly List<Page> _siteMaps = new List<Page>();
    
    public async Task LoadSiteMapsAsync()
    {
        string? html = await _siteMap.GetHtmlContent();
        if (html == null) return;
        string[] links = GetLinks(html);
        foreach (string link in links)
        {
            Console.WriteLine(link);
            _siteMaps.Add(new Page(link));
        }
    }

    public List<Page> GetPages()
    {
        return _siteMaps;
    }

    static string[] GetLinks(string html)
    {
        string pattern = @">https://[^\s<]+<";
        MatchCollection matches = Regex.Matches(html, pattern);
        List<string> links = new List<string>();

        foreach (Match match in matches)
        {
            // Remove the wrapping '>' and '<'
            string link = match.Value.Trim('>', '<');
            links.Add(link);
        }

        return links.ToArray();
    }
}
