using Models.Dtos;

namespace Servises.Interfaces
{
    public interface IAuthorizationService
    {
        Task<string> RegisterUserAsync(RegisterUserDto user);
        Task<LoginResponseDto> LoginUserAsync(LoginDto loginModel);
        Task<LoginResponseDto> LoginAdminAsync(LoginDto loginModel);
        Task<string> RegisterAdminAsync(RegisterAdminDto admin);

    }
}
