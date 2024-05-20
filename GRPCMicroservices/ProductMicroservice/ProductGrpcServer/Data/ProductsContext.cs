using Microsoft.EntityFrameworkCore;
using ProductGrpcServer.Models;

namespace ProductGrpcServer.Data;

public class ProductsContext : DbContext
{
    public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
    {
    }

    public DbSet<Product> Product { get; set; }
}