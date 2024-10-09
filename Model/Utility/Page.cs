

using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SaleCheck.Model.Utility;

public class Page(string title, string url, Page? parent = null)
{
    private string _title = title;
    private string? _htmlContent;
    private string? _fully_rendered_htmlContent;
    
    private List<string> hrefs = new List<string>();
    private List<ProductItem> products = new List<ProductItem>();
    
    
    private async Task<string?> LoadHtmlContentAsync()
    {
        HttpClientHandler handler = new HttpClientHandler { AllowAutoRedirect = true };
        HttpClient client = new HttpClient(handler);
        
        // Check if the URL starts with 'tel:'
        if (url.StartsWith("tel:")) return null;
        // Add headers to mimic a browser
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Language", "da-DK,da;q=0.5");

        int tries = 1;
        for (int i = 0; i < tries; i++)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: with {url}");
                Console.WriteLine(e.Message); 
            }
        }
        return "";
    }

    public async Task<string> LoadRenderedHtmlContentAsync()
    {
        // Set ChromeDriver options
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Run in headless mode
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");

        // Initialize WebDriver
        using (IWebDriver driver = new ChromeDriver(options))
        {
            // Navigate to the Elgiganten website
            driver.Navigate().GoToUrl(url);
            /*
            // Optionally handle cookies or pop-ups
            try
            {
                // Wait for the cookie consent button and click it
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement acceptCookiesButton = wait.Until(drv => drv.FindElement(By.Id("onetrust-accept-btn-handler")));
                acceptCookiesButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                return "No Cookies" + driver.PageSource; 
            }*/

            await Task.Delay(10000); // Optional: wait for 2 seconds to ensure the page is fully loaded

            // Return the rendered HTML content after clicking the button
            return driver.PageSource;
        }
    }



    private async Task LoadRenderedHtmlContentAsyncOld()
    {
        // Start a headless browser instance
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[] { "--disable-http2" }  // Disable HTTP/2 protocol
        });
        var page = await browser.NewPageAsync();

        try
        {
            // Navigate to the page
            await page.GotoAsync(url);

            // Wait for the dynamic content to load (you can customize this wait)
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Extract the fully rendered HTML content
            string content = await page.ContentAsync();
            await browser.CloseAsync();
            _fully_rendered_htmlContent = content;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {url}");
            Console.WriteLine(e.Message);
            await browser.CloseAsync();
            _fully_rendered_htmlContent = e.Message;
        }
    }
    
    public async Task<string?> GetHtmlContent()
    {
        if(_htmlContent == null) _htmlContent = await LoadHtmlContentAsync();
        return _htmlContent;
    }
    public async Task<string?> GetRenderedHtmlContent()
    {
        if(_fully_rendered_htmlContent == null) _fully_rendered_htmlContent = await LoadRenderedHtmlContentAsync();
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
        hrefs = new List<string>();
        var htmlDoc = new HtmlDocument();
        string? content = await GetHtmlContent();
        if (content == null) return;
        htmlDoc.LoadHtml(content);
        // Select all product cards using the data-testid attribute
        var hrefsFinds = htmlDoc.DocumentNode.SelectNodes("//a/@href");
        foreach (var href in hrefsFinds)
        {
            hrefs.Add(href.GetAttributeValue("href", "")); // Add the value of href attribute to the list        
        }
    }

    public async Task<List<ProductItem>> GetProducts()
    {
        if(products.Count == 0) await LoadProducts(ProductAnalyser.GetAnalyser(_title));
        return products;
    }

    private async Task LoadProducts(IProductAnalyser? analyser)
    {
        if (analyser == null)
        {
            Console.WriteLine("No analyser for this page");
            return;
        }
        products = await analyser.Analyze(this);
    }
    
    public string GetUrl()
    {
        return url;
    }
}


/*

        string pattern = @"<div class=""product-info-price"">.*?data-product-id=""(\d+)""";
        Match match = Regex.Match(content, pattern);
        string productId = match.Success ? match.Groups[1].Value : "undefined";
        
        string patternCategory = @"<div class=""page-title"">.*?<h1 class=""product-category"" itemprop=""name"">\s*(.*?)\s*</h1>"; //category
        string patternSku = @"<meta itemprop=""sku"" content=""(.*?)"""; //name


        // Find the product category using the regex pattern
        Match matchCategory = Regex.Match(content, patternCategory, RegexOptions.Singleline);
        string? productCategory = matchCategory.Success ? matchCategory.Groups[1].Value.Trim() : null;

        // Find the product Name using the regex pattern
        Match matchName = Regex.Match(content, patternSku, RegexOptions.Singleline);
        string? productName = matchName.Success ? matchName.Groups[1].Value.Trim() : null;

        string productFullName = productCategory != null && productName != null ? $"{productCategory} ({productName})" : "undefined";

        string oldPrice = "not found";
        string newPrice = "not found";
        
*/