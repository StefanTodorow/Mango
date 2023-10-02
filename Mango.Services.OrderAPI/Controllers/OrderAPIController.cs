using AutoMapper;
using Mango.OrderAPI.Utility;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private IProductService _productService;
        protected ResponseDTO _response;

        public OrderAPIController(AppDbContext db,
            IProductService productService, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                OrderHeader newOrderHeader = (await _db.OrderHeaders.AddAsync(_mapper.Map<OrderHeader>(orderHeaderDTO)))
                    .Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = newOrderHeader.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [ActionName("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = "https://example.com/success",
                    LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                Price = "price_H5ggYwtDq4fbrJ",
                                Quantity = 2
                            }
                        },
                    Mode = "payment"
                };

                var service = new SessionService();
                service.Create(options);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
