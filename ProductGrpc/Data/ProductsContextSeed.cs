using ProductGrpc.Models;

namespace ProductGrpc.Data
{
    public class ProductsContextSeed
    {
        public static void SeedAsync(ProductsContext productsContext)
        {
            if(!productsContext.Product.Any())
            {
                var products = new List<Product>
                {
                    new Product {
                        ProductId = 1,
                        Name = "Mi10T",
                        Description = "New Xiaomi Phone Mi10T",
                        Price = 699,
                        Status = ProductGrpc.Models.ProductStatus.INSTOCK,
                        CreatedTime = DateTime.Now
                    },
                    new Product {
                        ProductId = 2,
                        Name = "P40",
                        Description = "New Xiaomi Phone P40",
                        Price = 899,
                        Status = ProductGrpc.Models.ProductStatus.INSTOCK,
                        CreatedTime = DateTime.Now
                    }
                };

                productsContext.Product.AddRange(products);
                productsContext.SaveChanges();
            }
            
        }
    }
}
