using Models.Dtos;

namespace Servises.Interfaces
{
    public interface IAuthorizationService
    {
        Task<string> RegisterUserAsync(RegisterUserDto user);
        Task<LoginResponseDto> LoginAsync(LoginDto loginModel);
        Task<string> RegisterAdminAsync(RegisterAdminDto admin);
        Task<LoginResponseDto> Refresh(RefreshDto refreshModel);
        Task<string> UserForgotPassword(string email);
        Task UserResetPassword(ResetPasswordDto model);
    }
}
