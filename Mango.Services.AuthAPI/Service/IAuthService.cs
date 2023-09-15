using Mango.Services.AuthAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Service
{
    public interface IAuthService
    {
        Task<UserDTO> Register(RegistrationRequestDTO requestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO);
    }
}
