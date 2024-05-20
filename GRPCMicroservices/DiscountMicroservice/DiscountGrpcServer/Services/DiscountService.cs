using DiscountGrpcServer.Data;
using DiscountGrpcServer.Protos;
using Grpc.Core;

namespace DiscountGrpcServer.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(ILogger<DiscountService> logger)
    {
        _logger = logger;
    }

    public override Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var discount = DiscountContext.Discounts.FirstOrDefault(s => s.Code == request.DiscountCode);
        
        _logger.LogInformation("Discount is operated with the {discountCode} code and the amount is : {discountAmount}", discount.Code, discount.Amount);

        return Task.FromResult(new DiscountModel
        {
            DiscountId = discount.DiscountId,
            Code = discount.Code,
            Amount = discount.Amount
        });
    }
}