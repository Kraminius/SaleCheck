namespace SaleCheck.Model.Utility;

public static class PageLoading
{
        public static async Task ProcessPageToDictionaryAsync(Page page, Dictionary<string, ProductItem> products)
        {
            List<ProductItem> pageProducts = await page.GetProducts();

            if (pageProducts.Count > 0)
            {
                foreach (ProductItem product in pageProducts)
                {
                    products.TryAdd(product.Id, product);
                }
            }
        }
}