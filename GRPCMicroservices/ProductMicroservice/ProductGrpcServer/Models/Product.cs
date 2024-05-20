namespace ProductGrpcServer.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime CreatedTime { get; set; }
}

public enum ProductStatus
{
    INSTOCK = 0,
    LOW = 1,
    NONE = 2
}