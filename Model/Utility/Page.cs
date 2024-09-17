

namespace SaleCheck.Model.Utility;

public class Page(string url)
{
    private string? _htmlContent;
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

    public string GetUrl()
    {
        return url;
    }
}