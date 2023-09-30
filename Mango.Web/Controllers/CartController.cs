using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims
                .Where(c => c.Type == JwtRegisteredClaimNames.Sub)?
                .FirstOrDefault()?
                .Value;

            var responseDTO = await _cartService.RemoveFromCartAsync(cartDetailsId);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Shopping Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            var userId = User.Claims
                .Where(c => c.Type == JwtRegisteredClaimNames.Sub)?
                .FirstOrDefault()?
                .Value;

            var responseDTO = await _cartService.ApplyCouponAsync(cartDTO);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Shopping Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = string.Empty;

            var userId = User.Claims
                .Where(c => c.Type == JwtRegisteredClaimNames.Sub)?
                .FirstOrDefault()?
                .Value;

            var responseDTO = await _cartService.ApplyCouponAsync(cartDTO);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Shopping Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            var responseDTO = await _cartService.EmailCartAsync(cartDTO);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
        {
            var userId = User.Claims
                .Where(c => c.Type == JwtRegisteredClaimNames.Sub)?
                .FirstOrDefault()?
                .Value;

            var responseDTO = await _cartService.GetCartByUserIdAsync(userId);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                var cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(responseDTO.Result));
                return cartDTO;
            }

            return new CartDTO();
        }
    }
}
