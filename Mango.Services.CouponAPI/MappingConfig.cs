using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(conf =>
            {
                conf.CreateMap<Coupon, CouponDTO>();
                conf.CreateMap<CouponDTO, Coupon>();
            });

            return mappingConfig;
        }
    }
}
