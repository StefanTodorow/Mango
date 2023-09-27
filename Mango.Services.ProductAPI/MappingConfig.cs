using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<Product, ProductDTO>();
                conf.CreateMap<ProductDTO, Product>();
            });

            return mappingConfig;
        }
    }
}
