using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SaleCheck.Model.Utility;

public class Page(string title, string url, Page? parent = null, int retryCount = 1)
{
    private static readonly HttpClientHandler handler = new HttpClientHandler
    {
        AllowAutoRedirect = true,
        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
    };

    private static readonly HttpClient client = new HttpClient(handler)
    {
        Timeout = TimeSpan.FromSeconds(600)
    };

    static Page()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        // Mimic `curl` headers
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    private readonly string _title = title;
    private readonly string url = url;
    private string? _fully_rendered_htmlContent;
    private readonly List<string> hrefs = new List<string>();
    private readonly List<ProductItem> products = new List<ProductItem>();

    /// <summary>
    /// Fetch raw HTML content using a shared HttpClient instance.
    /// </summary>
    private async Task<string?> LoadHtmlContentAsync()
    {
        for (int i = 0; i < retryCount; i++) // Retry logic
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Non-success status code ({(int)response.StatusCode}) received. Retrying...");
                    continue; // Skip to the next iteration
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) when (e.InnerException is IOException)
            {
                Console.WriteLine($"Network error on attempt {i + 1}: {e.InnerException?.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"General error on attempt {i + 1}: {e.Message}");
            }

            await Task.Delay(2000); // Retry delay
        }

        throw new Exception($"Failed to fetch {url} after {retryCount} retries.");
    }

    /// <summary>
    /// Fetch rendered HTML content using Selenium ChromeDriver (for JavaScript-rendered pages).
    /// </summary>
    public async Task<string> LoadRenderedHtmlContentAsync()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");

        using (IWebDriver driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl(url);
            await Task.Delay(10000); // Wait for the page to load
            return driver.PageSource;
        }
    }

    /// <summary>
    /// Alternative: Use Playwright for rendering JavaScript content.
    /// </summary>
    private async Task LoadRenderedHtmlContentAsyncOld()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[] { "--disable-http2" } // Disable HTTP/2 if needed
        });

        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            _fully_rendered_htmlContent = await page.ContentAsync();
        }
        finally
        {
            await browser.CloseAsync();
        }
    }

    public async Task<string?> GetHtmlContent()
    {
        return await LoadHtmlContentAsync();
    }

    public async Task<string?> GetRenderedHtmlContent()
    {
        if (_fully_rendered_htmlContent == null)
        {
            _fully_rendered_htmlContent = await LoadRenderedHtmlContentAsync();
        }
        return _fully_rendered_htmlContent;
    }

    public async Task<List<string>> GetHrefs()
    {
        if (hrefs.Count == 0)
        {
            await LoadHrefs();
        }
        return hrefs;
    }

    public async Task LoadHrefs()
    {
        hrefs.Clear();
        var htmlDoc = new HtmlDocument();
        string? content = await GetHtmlContent();
        if (content == null) return;

        htmlDoc.LoadHtml(content);
        var hrefsFinds = htmlDoc.DocumentNode.SelectNodes("//a/@href");
        if (hrefsFinds != null)
        {
            foreach (var href in hrefsFinds)
            {
                hrefs.Add(href.GetAttributeValue("href", ""));
            }
        }
    }

    public async Task<List<ProductItem>> GetProducts()
    {
        if (products.Count == 0)
        {
            await LoadProducts(ProductAnalyser.GetAnalyser(_title));
        }
        return products;
    }

    private async Task LoadProducts(IProductAnalyser? analyser)
    {
        if (analyser == null)
        {
            Console.WriteLine("No analyser for this page");
            return;
        }
        products.Clear();
        products.AddRange(await analyser.Analyze(this));
    }

    public string GetUrl()
    {
        return url;
    }
}
