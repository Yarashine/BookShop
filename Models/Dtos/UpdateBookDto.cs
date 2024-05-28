﻿using Models.Entities;

namespace Models.Dtos;

public record UpdateBookDto(string Title, string? Description, List<string>? Tags, List<string>? Genres,
                    MediaCreationDto? ImageDto, MediaCreationDto? EBookDto,
                      string? Series, int? Price);
