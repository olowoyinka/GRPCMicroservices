using AutoMapper;
using ShoppingCartGrpcServer.Models;
using ShoppingCartGrpcServer.Protos;

namespace ShoppingCartGrpcServer.Mapper;

public class ShoppingCartProfile : Profile
{
    public ShoppingCartProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartModel>().ReverseMap();
        CreateMap<ShoppingCartItem, ShoppingCartItemModel>().ReverseMap();
    }
}