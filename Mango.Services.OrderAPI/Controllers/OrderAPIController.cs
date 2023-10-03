using AutoMapper;
using Mango.MessageBus;
using Mango.OrderAPI.Utility;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        private IProductService _productService;
        protected ResponseDTO _response;

        public OrderAPIController(AppDbContext db, IProductService productService, 
            IMapper mapper, IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDTO();
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDTO? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;

                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.Include(oh => oh.OrderDetails)
                        .OrderByDescending(oh => oh.OrderHeaderId).ToList();
                }
                else
                {
                    objList = _db.OrderHeaders.Include(oh => oh.OrderDetails).Where(oh => oh.UserId == userId)
                        .OrderByDescending(oh => oh.OrderHeaderId).ToList();
                }

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDTO? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(oh => oh.OrderDetails)
                    .First(oh => oh.OrderHeaderId == id);

                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(oh => oh.OrderHeaderId == orderId);

                if (orderHeader != null)
                {
                    if (newStatus == SD.Status_Cancelled)
                    { //we must give a refund
                        var refundOptions = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(refundOptions);
                    }

                    orderHeader.Status = newStatus;
                }
            }
            catch (Exception)
            {
                _response.IsSuccess = false;
            }

            return _response;
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
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Quantity
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);

                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders
                    .First(oh => oh.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;

                _db.SaveChanges();
                _response.Result = stripeRequestDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(oh => oh.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentService.Get(orderHeader.StripeSessionId);

                if (paymentIntent.Status == "succeeded")
                { //payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;

                    _db.SaveChanges();

                    RewardDTO rewardDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardDTO, topicName);

                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
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
