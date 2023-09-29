﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db = null;
        private IMapper _mapper = null;
        private ResponseDTO _response = null;

        public CartAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDTO();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders
                    .FirstOrDefaultAsync(ch => ch.UserId == cartDTO.CartHeader.UserId);

                if (cartHeaderFromDb is null)
                {
                    //create new header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails
                        .FirstOrDefaultAsync(cd => cd.ProductId == cartDTO.CartDetails.First().ProductId &&
                        cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb is null)
                    {
                        //create one
                    }
                    else
                    {
                        //update quantity in cart details
                    }
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
        }
    }
}
