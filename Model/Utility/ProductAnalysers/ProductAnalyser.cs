using SaleCheck.Model.Utility.ProductAnalysers;

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
            case "sportyfit":
                return new SportyFitAnalyser();
            default:
                return null;
        }
    }
}