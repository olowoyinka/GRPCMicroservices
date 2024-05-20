namespace DiscountGrpcServer.Models;

public class Discount
{
    public int DiscountId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Amount { get; set; }
}