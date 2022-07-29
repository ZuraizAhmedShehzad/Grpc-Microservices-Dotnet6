// See https://aka.ms/new-console-template for more information


using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;

Console.WriteLine("Waiting for server...");
Thread.Sleep(5000);

using var channel = GrpcChannel.ForAddress("http://localhost:5237");
var client = new ProductProtoService.ProductProtoServiceClient(channel);

await GetProductAsync(client);
await GetAllProductsAsync(client);
await AddProductAsync(client);
await UpdateProductAsync(client);
await DeleteProductAsync(client);
await InsertBulkProductAsync(client);
await GetAllProductsAsync(client);
Console.ReadLine();

async Task InsertBulkProductAsync(ProductProtoService.ProductProtoServiceClient client)
{
    Console.WriteLine("InsertBulkProductAsync Started...");
    using var clientBulk = client.InsertBulkProduct();
    
    for(int i = 0; i < 3; i++)
    {
        var productModel = new ProductModel
        {
            Name = $"Product{i}",
            Description = "Bulk Inserted Product",
            Price = 399,
            Status = ProductStatus.Instock,
            CreatedTime = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc))
        };

        await clientBulk.RequestStream.WriteAsync(productModel);
    }

    await clientBulk.RequestStream.CompleteAsync();

    var responseBulk = await clientBulk;
    Console.WriteLine($"Status: {responseBulk.Success}. Insert Count: {responseBulk.InsertCount}");
}

static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
{
    //GetProductAsync
    Console.WriteLine("GetProductAsync Started...");
    var response = await client.GetProductAsync(
        new GetProductRequest
        {
            ProductID = 1
        });

    Console.WriteLine("GetProductAsync Response :" + response.ToString());
}

static async Task GetAllProductsAsync(ProductProtoService.ProductProtoServiceClient client)
{
    Console.WriteLine("GetAllProducts with C# 9 started...");
    using var clientData = client.GetAllProducts(new GetAllProductsRequest());
    await foreach (var responseData in clientData.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(responseData);
    }
}

static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
{
    Console.WriteLine("AddProductAsync Started ...");
    var addProductResponse = await client.AddProductAsync(
        new AddProductRequest
        {
            Product = new ProductModel
            {
                Name = "Red",
                Description = "New Red Phone Mi10T",
                Price = 699,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc))
            }
        }
     );

    Console.WriteLine("AddProduct Response: " + addProductResponse.ToString());
}

async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
{
    Console.WriteLine("UpdateProductAsync Started ...");
    var updateProductRequest = await client.UpdateProductAsync(
        new UpdateProductRequest
        {
            Product = new ProductModel
            {
                ProductId = 1,
                Name = "Red",
                Description = "New Red Phone Mi10T",
                Price = 699,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc))
            }
        }
    ); 

    Console.WriteLine("UpdateProductAsync Response: " + updateProductRequest.ToString());
}

async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
{
    Console.WriteLine("DeleteProductAsync Started ...");
    var deleteProductRequest = await client.DeleteProductAsync(
            new DeleteProductRequest
            {
                ProductID = 3,
            }
        );
    Console.WriteLine("DeleteProductAsync Response: " + deleteProductRequest.Success.ToString());
    Thread.Sleep(1000);
}


