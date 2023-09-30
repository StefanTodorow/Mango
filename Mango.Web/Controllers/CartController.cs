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
