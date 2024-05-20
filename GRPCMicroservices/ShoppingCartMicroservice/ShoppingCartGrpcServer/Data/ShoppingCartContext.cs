using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpcServer.Models;

namespace ShoppingCartGrpcServer.Data;

public class ShoppingCartContext : DbContext
{
    public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
    {
    }
    
    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
}