namespace Models.Dtos;

public record LoginResponseDto(
    string JwtToken,
    DateTime Expiration,
    string RefreshToken,
    Guid Id);
