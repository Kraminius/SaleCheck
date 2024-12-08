namespace SaleCheck.Model.Utility;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SiteMap(string title, string url)
{
    private readonly Page _siteMap = new Page(title, url);
    private readonly List<Page> _siteMaps = new List<Page>();
    
    public async Task LoadSiteMapsAsync()
    {
        string? html = await _siteMap.GetHtmlContent();
        if (html == null) return;
        IProductAnalyser analyser = ProductAnalyser.GetAnalyser(title);
        if (analyser == null) return;
        
        List<Page> links = await analyser.GetSitemapLinks(_siteMap);
        _siteMaps.AddRange(links);
    }

    public string GetLink()
    {
        return url;
    }

    public List<Page> GetPages()
    {
        return _siteMaps;
    }

    
}
