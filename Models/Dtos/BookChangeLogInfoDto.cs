using Models.Entities;

namespace Models.Dtos;

public record BookChangeLogInfoDto(Guid Id,string AuthorName, string? Description, DateTime DateCreated);

