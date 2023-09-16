using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(AppDbContext db,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> Register(RegistrationRequestDTO requestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,
                NormalizedEmail = requestDTO.Email.ToUpper(),
                Name = requestDTO.Name,
                PhoneNumber = requestDTO.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, requestDTO.Password);

                if (result.Succeeded)
                    return string.Empty;
                else
                    return result.Errors.FirstOrDefault().Description;
            }
            catch
            {
            }

            return "There was a problem with the registration.";
        }

        public Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
