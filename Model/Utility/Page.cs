

using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SaleCheck.Model.Utility;

public class Page(string url, Page? parent = null)
{
    private string? _htmlContent;
    
    private List<Page> hrefs = new List<Page>();
    private Product? product = null;
    private async Task<string?> LoadHtmlContentAsync()
    {
        HttpClientHandler handler = new HttpClientHandler { AllowAutoRedirect = true };
        HttpClient client = new HttpClient(handler);
        {
            // Check if the URL starts with 'tel:'
            if (url.StartsWith("tel:")) return null;
            // Add headers to mimic a browser
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "da-DK,da;q=0.5");

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
                hrefs.Add(new Page(url, this));
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

    public async Task<Product?> GetProduct()
    {
        if(product == null) await LoadProduct();
        return product;
    }

    private async Task LoadProduct()
    {
        string? content = await GetHtmlContent();
        if(content == null) return;
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(content);
        //FINDING ID
        string pattern = @"<div class=""product-info-price"">.*?data-product-id=""(\d+)""";
        Match match = Regex.Match(content, pattern);
        string productId = match.Success ? match.Groups[1].Value : "undefined";
        //FINDING PRICE
        string shownPrice = "no ids found"; //Default value if none is found
        string? otherPrice = "no ids found";
        string xpathQuery = "//*[@class='price-wrapper ']/*[contains(@class,'price')]";
        var nodeList = doc.DocumentNode.SelectNodes(xpathQuery);
        if (nodeList != null && nodeList.Count > 0)
        {
            shownPrice = nodeList[0].InnerText; //Gets the Inner Text of the first matching element
            otherPrice = nodeList[0].GetAttributeValue("attr-price", null) ?? "none"; //Gets the attribute value of "attr-price" if it exists, otherwise null
        }
        //FINDING NAME
        string patternCategory = @"<div class=""page-title"">.*?<h1 class=""product-category"" itemprop=""name"">\s*(.*?)\s*</h1>"; //category
        string patternSku = @"<meta itemprop=""sku"" content=""(.*?)"""; //name
        Match matchCategory = Regex.Match(content, patternCategory, RegexOptions.Singleline);
        string? productCategory = matchCategory.Success ? matchCategory.Groups[1].Value.Trim() : null;
        Match matchName = Regex.Match(content, patternSku, RegexOptions.Singleline);
        string? productName = matchName.Success ? matchName.Groups[1].Value.Trim() : null;
        string productFullName = productCategory != null && productName != null ? $"{productCategory} ({productName})" : "undefined";
        //CREATE PRODUCT
        if (productId == "undefined") return; //No product? don't add it. makes sense.
        if (otherPrice == "none") otherPrice = null;
        if(otherPrice == null) product = new Product(url, productFullName, productId, PriceParse(shownPrice));
        else product = new Product(url, productFullName, productId, PriceParse(otherPrice), PriceParse(shownPrice));
    }
    public static double PriceParse(string price)
    {
        var numericChars = new Regex(@"[^0-9,]", RegexOptions.Compiled);
        var removeComma = new Regex(@"[.]", RegexOptions.Compiled);
        string cleanedPrice = price.Contains("kr") ? numericChars.Replace(price, ""): removeComma.Replace(price, ",");
        cleanedPrice = cleanedPrice.Trim();
        if (double.TryParse(cleanedPrice, out double result))        {
            return result;
        }
        else
        {
            throw new ArgumentException("Invalid price format: " + cleanedPrice);
        }
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