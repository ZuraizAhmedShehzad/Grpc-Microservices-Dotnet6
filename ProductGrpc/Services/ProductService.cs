using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductGrpc.Data;
using ProductGrpc.Models;
using ProductGrpc.Protos;

namespace ProductGrpc.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductsContext _productsContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductsContext productsContext, ILogger<ProductService> logger, IMapper mapper)
        {
            _productsContext = productsContext ?? throw new ArgumentNullException(nameof(productsContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _productsContext.Product.FindAsync(request.ProductID);
            if (product == null)
            {
                //throw an RPC exception
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"Product with ID={request.ProductID} is not found"));
            }

            var productModel = _mapper.Map<ProductModel>(product);

            return productModel;
        }

        public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
        {
            var productList = await _productsContext.Product.ToListAsync();
            
            foreach(var product in productList)
            {
                var productModel = _mapper.Map<ProductModel>(product);
                await responseStream.WriteAsync(productModel);
            }

        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);

            _productsContext.Product.Add(product);
            await _productsContext.SaveChangesAsync();

            _logger.LogInformation("Product successfully added: {productID}_{productName}", product.ProductId.ToString(), product.Name);

            var productModel = _mapper.Map<ProductModel>(product);

            return productModel;
        }

        public override async Task<ProductModel> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);

            bool isExist = await _productsContext.Product.AnyAsync(p => p.ProductId == product.ProductId);
            if(!isExist)
            {
                // throw RPC Exception
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Product with ID={product.ProductId} is not found"));
            }
            _productsContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _productsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }

        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            var product = await _productsContext.Product.FindAsync(request.ProductID);
            if(product == null)
            {
                // throw an RPC exception
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Product with ID={product.ProductId} is not found"));
            }

            _productsContext.Product.Remove(product);
            var deleteCount = await _productsContext.SaveChangesAsync();
            var response = new DeleteProductResponse{
                Success = deleteCount > 0
            };

            return response;
        }

        public override async Task<InsertBulkProductResponse> InsertBulkProduct(IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var product = _mapper.Map<Product>(requestStream.Current);
                _productsContext.Product.Add(product);
            }

            var insertCount = await _productsContext.SaveChangesAsync();

            var response = new InsertBulkProductResponse
            {
                Success = insertCount > 0,
                InsertCount = insertCount
            };

            return response;
        }
    }
}
