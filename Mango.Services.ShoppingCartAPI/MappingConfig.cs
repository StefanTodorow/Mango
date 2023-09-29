using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<CartHeader, CartHeaderDTO>();
                conf.CreateMap<CartHeaderDTO, CartHeader>();
                conf.CreateMap<CartDetails, CartDetailsDTO>();
                conf.CreateMap<CartDetailsDTO, CartDetails>();
            });

            return mappingConfig;
        }
    }
}
