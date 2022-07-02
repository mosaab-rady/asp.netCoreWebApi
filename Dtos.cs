using System.ComponentModel.DataAnnotations;

namespace WebApi.Api.Dtos;
public record ItemDto(Guid Id, string? Name, decimal Price, DateTimeOffset createdDate);
public record CreateItemDto([Required] string Name, [Range(1, 1000)] decimal Price);
public record UpdateItemDto([Required] string Name, [Range(1, 1000)] decimal Price);

