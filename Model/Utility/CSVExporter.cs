using System.Text;

namespace SaleCheck.Model.Utility;

public static class CSVExporter
{
    public static bool SaveProductsToCsv(Dictionary<string, ProductItem> products, string filePath)
    {
        var csvContent = new StringBuilder();
        csvContent.AppendLine("Url;Name;Id;Price;OtherPrice"); //Header to know what is in each cell
        foreach (var kvp in products)
        {
            var product = kvp.Value;
            var line = $"{EscapeCsvValue(product.Url)};{EscapeCsvValue(product.Name)};{EscapeCsvValue(product.Id)};{product.Price};{(product.OtherPrice.HasValue ? product.OtherPrice.Value.ToString() : "N/A")}";
            csvContent.AppendLine(line);
        }

        try
        {
            File.WriteAllText(filePath, csvContent.ToString());
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        
    }

    private static string EscapeCsvValue(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            // Escape quotes
            value = value.Replace("\"", "\"\"");
            // Wrap in quotes
            value = $"\"{value}\"";
        }
        return value;
    }
}
