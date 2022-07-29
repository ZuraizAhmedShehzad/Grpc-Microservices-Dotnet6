using DiscountGrpc.Data;
using DiscountGrpc.Model;
using DiscountGrpc.Protos;
using Grpc.Core;

namespace DiscountGrpc.Services
{
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _logger;
        
        public DiscountService(ILogger<DiscountService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var discount = DiscountContext.Discounts.FirstOrDefault(d => d.Code == request.DiscountCode);

            _logger.LogInformation("Discount is operated with the {discountCode} code and " +
                "the amount is : {discountAmount}", discount.Code, discount.Amount);

            return Task.FromResult(new DiscountModel
            {
                DiscountId = discount.DiscountId,
                Code = discount.Code,
                Amount = discount.Amount
            });
        }
    }
}
