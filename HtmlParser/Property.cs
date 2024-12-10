
namespace SaleCheck.HtmlLib;

public class Property(string type, string value)
{
    public string Type { get; init; } = type;
    public string Value { get; init; } = value;
    
    public override string ToString()
    {
        return Type + "[" + Value + "]";
    }
}