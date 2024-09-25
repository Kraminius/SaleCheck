

using System.Text.RegularExpressions;

namespace SaleCheck.Model.Utility;

public class Page(string url)
{
    private string? _htmlContent;
    
    private List<Page> hrefs = new List<Page>();
    private async Task<string> LoadHtmlContentAsync()
    {
        HttpClientHandler handler = new HttpClientHandler { AllowAutoRedirect = true };
        HttpClient client = new HttpClient(handler);
        {
            // Add headers to mimic a browser
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: with {url}");
                Console.WriteLine(e.Message);
                return "";
            }
        }
    }
    public async Task<string?> GetHtmlContent()
    {
        if (_htmlContent == null) _htmlContent = await LoadHtmlContentAsync();
        return _htmlContent;
    }

    public async Task<List<Page>> GetHrefs()
    {
        if (hrefs.Count == 0)
        {
            List<string> urls = new List<string>();
            urls = await LoadHrefs();
            foreach (string url in urls)
            {
                hrefs.Add(new Page(url));
            }
        }
        return hrefs;
    }

    public async Task<List<string>> LoadHrefs()
    {
        hrefs = new List<Page>();
        string? content = await GetHtmlContent();
        if (content == null) return new List<string>();

        // Regular expression to match href links
        string pattern = @"href\s*=\s*[""']([^""']+)[""']";
        
        MatchCollection matches = Regex.Matches(content, pattern);
        Console.WriteLine($"Found {matches.Count} matches");
        List<string> links = new List<string>();
        foreach (Match match in matches)
        {
            string url = match.Groups[1].Value;
            links.Add(url);
        }

        return links;
    }

    public string GetUrl()
    {
        return url;
    }
}