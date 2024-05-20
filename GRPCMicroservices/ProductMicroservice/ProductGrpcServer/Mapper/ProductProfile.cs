using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpcServer.Models;
using ProductGrpcServer.Protos;

namespace ProductGrpcServer.Mapper;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductModel>()
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedTime)));

        CreateMap<ProductModel, Product>()
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime.ToDateTime()));
    }
}
