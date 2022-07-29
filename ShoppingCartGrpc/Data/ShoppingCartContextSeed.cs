using ShoppingCartGrpc.Models;

namespace ShoppingCartGrpc.Data
{
    public class ShoppingCartContextSeed
    {
        public static void SeedAsync(ShoppingCartContext shoppingCartContext)
        {
            if (!shoppingCartContext.ShoppingCart.Any())
            {
                var shoppingCart = new List<ShoppingCart>
                {
                    new ShoppingCart {
                        UserName = "swn",
                        Items = new List<ShoppingCartItem> {
                            new ShoppingCartItem {
                                Quantity = 2,
                                Color = "Black",
                                Price = 699,
                                ProductId = 1,
                                ProductName = "Mi10T"
                            },
                            new ShoppingCartItem
                            {
                                Quantity = 3,
                                Color = "Red",
                                Price = 899,
                                ProductId= 2,
                                ProductName = "P40"
                            }
                        }
                    }                  
                };

                shoppingCartContext.ShoppingCart.AddRange(shoppingCart);
                shoppingCartContext.SaveChanges();
            }

        }
    }
}
