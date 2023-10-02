using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                    .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                conf.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
                    .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                conf.CreateMap<OrderDetailsDTO, CartDetailsDTO>();
                conf.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
                conf.CreateMap<OrderDetailsDTO, OrderDetails>();
            });

            return mappingConfig;
        }
    }
}
