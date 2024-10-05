namespace SaleCheck.Model.Utility;

public static class PageLoading
{
        public static async Task ProcessPageToDictionaryAsync(Page page, Dictionary<string, Product> products)
        {
            List<Product> pageProducts = await page.GetProducts();

            if (pageProducts.Count > 0)
            {
                foreach (Product product in pageProducts)
                {
                    products.TryAdd(product.Id, product);
                }
            }
        }
}