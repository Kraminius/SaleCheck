﻿using System.Text.RegularExpressions;

namespace SaleCheck.Model.Utility;

public class Robots(string url)
{
    private List<SiteMap> _siteMaps = new List<SiteMap>();
    public async Task<List<SiteMap>> GetSiteMaps()
    {
        if (_siteMaps.Count == 0) await LoadSiteMaps();
        return _siteMaps;
    }
    
    private async Task LoadSiteMaps()
    {
        Page robots = new Page(url + "robots.txt");
        string? html = await robots.GetHtmlContent();
        if (html == null) return; //returns if none is found
        string[] sitemapLinks = RegexMatchLinksFromHtml(html);
        _siteMaps = new List<SiteMap>();
        foreach (string sitemapLink in sitemapLinks)
        {
            _siteMaps.Add(new SiteMap(sitemapLink));
        }
    }
    
    private string[] RegexMatchLinksFromHtml(string html)
    {
        string pattern = @"Sitemap:\s*(https?://\S+|/\S+)";
        MatchCollection matches = Regex.Matches(html, pattern);
        List<string> sitemapLinks = new List<string>();

        foreach (Match match in matches)
        {
            string link = match.Groups[1].Value;
            
            /*
             Some robots.txt write their sitemap as /sitemap.xsl, 
             while others use the whole link as https://website.dk/sitemap.xml
             we therefore add the base url to the front of /sitemap to make sure we get links that can be used.
             */
            if (!link.StartsWith("http")) 
            {
                link = url.TrimEnd('/') + link;
            }

            // Skip link if it is not danish, other countries is not within our scope. (Only does something for pages who sell globally)
            if (!link.Contains(".dk") && !link.Contains("da_dk"))
            {
                continue; 
            }

            sitemapLinks.Add(link);
        }

        return sitemapLinks.ToArray();
    }

    public async Task<List<Page>> GetAllOriginalPages()
    {
        List<string> allPageURls = new List<string>();
        List<SiteMap> sitemaps = await GetSiteMaps();
        foreach (SiteMap siteMap in sitemaps)              
        {
            await siteMap.LoadSiteMapsAsync();
            List<Page> siteMapPages = siteMap.GetPages();
            foreach (Page page in siteMapPages)
            {
                if(!allPageURls.Contains(page.GetUrl())) 
                    allPageURls.Add(page.GetUrl());
                string htmlContent = await page.GetHtmlContent();
                if (htmlContent == null) continue;
                List<Page> hrefs = await page.GetHrefs();
                // TODO scrape the html for useful products and save them to a database.
                foreach (Page href in hrefs)
                {
                    if(!allPageURls.Contains(href.GetUrl())) 
                        allPageURls.Add(href.GetUrl());
                }
            }
        }
        List<Page> allPages = new List<Page>();
        foreach (string pageUrl in allPageURls)
        {
            allPages.Add(new Page(pageUrl));
        }
        return allPages;
    }
}