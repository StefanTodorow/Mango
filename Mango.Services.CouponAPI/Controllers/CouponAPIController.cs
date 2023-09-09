using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db = null;

        public CouponAPIController(AppDbContext db)
        {
                _db = db;
        }

        [HttpGet]   
        public object Get()
        {
            try
            {
                return _db.Coupons.ToList();
            }
            catch
            {
            }

            return null;
        }

        [HttpGet]
        [Route("{id:int}")]
        public object Get(int id)
        {
            try
            {
                return _db.Coupons.First(c => c.CouponId == id);
            }
            catch
            {
            }

            return null;
        }
    }
}
