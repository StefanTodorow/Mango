using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db = null;
        private IMapper _mapper = null;
        private ResponseDTO _response = null;
        private IProductService _productService;

        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new();
                cart.CartHeader = _mapper.Map<CartHeaderDTO>(_db.CartHeaders.First(ch => ch.UserId == userId));
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_db.CartDetails
                    .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> productDTOs = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDTOs.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Quantity * item.Product.Price);
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(ch => ch.UserId == cartDTO.CartHeader.UserId);

                if (cartHeaderFromDb is null)
                {   //create new header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);

                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;

                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {   //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(cd => cd.ProductId == cartDTO.CartDetails.First().ProductId &&
                        cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb is null)
                    {   //add new cart details
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;

                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {   //update quantity in cart details
                        cartDTO.CartDetails.First().Quantity += cartDetailsFromDb.Quantity;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails
                    .First(cd => cd.CartDetailsId == cartDetailsId);

                int totalQuantityOfCartItem = _db.CartDetails
                    .Where(cd => cd.CartHeaderId == cartDetails.CartHeaderId)
                    .Count();

                _db.CartDetails.Remove(cartDetails);

                if (totalQuantityOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(ch => ch.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }
    }
}
