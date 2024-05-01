using Models.Entities;
namespace Models.Dtos;

public record ResetPasswordDto(string Email, string NewPassword, string ResetToken);

