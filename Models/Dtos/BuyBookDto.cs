using Models.Entities;

namespace Models.Dtos;

public record BuyBookDto(Guid Id, string Title, decimal Price);
