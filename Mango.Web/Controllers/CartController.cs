using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
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

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();

            if (ModelState.IsValid)
            {
                cart.CartHeader.Name = cartDTO.CartHeader.Name;
                cart.CartHeader.PhoneNumber = cartDTO.CartHeader.PhoneNumber;
                cart.CartHeader.Email = cartDTO.CartHeader.Email;

                var response = await _orderService.CreateOrderAsync(cart);
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

                if (response != null && response.IsSuccess)
                { //Get Stripe session and redirect to Stripe to place order
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                    StripeRequestDTO stripeRequestDTO = new()
                    {
                        ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDTO.OrderHeaderId,
                        CancelUrl = domain + "cart/checkout",
                        OrderHeader = orderHeaderDTO
                    };

                    var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDTO);
                    StripeRequestDTO stripeResponseResult = JsonConvert
                        .DeserializeObject<StripeRequestDTO>(Convert.ToString(response.Result));
                    Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);

                    return View("Confirmation", cart.CartHeader.CartHeaderId);
                }
            }

            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);

            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert
                    .DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

                if (orderHeaderDTO.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            else
            { //Redirect to error page

            }

            return View(orderId);
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
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();

            cart.CartHeader.Email = User.Claims
                .Where(c => c.Type == JwtRegisteredClaimNames.Email)?
                .FirstOrDefault()?
                .Value;

            var responseDTO = await _cartService.EmailCartAsync(cart);

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
