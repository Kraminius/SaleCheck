namespace SaleCheck.Model.Utility;

public static class ProductAnalyser
{
    public static IProductAnalyser? GetAnalyser(string title)
    {
        switch (title)
        {
            case "vinduesgrossisten":
                return new VinduesGrossistenAnalyser();
            case "elgiganten":
                return new ElgigantenAnalyser();
            default:
                return null;
        }
    }
}