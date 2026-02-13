using System.Text.Json.Serialization;

namespace RandomUser.src.Infrastructure.Http.DTOs;

// DTOs para deserializar la respuesta de la API externa

public sealed record RandomUserResult
{
  [JsonPropertyName("phone")]
  public required string Phone { get; init; }

  [JsonPropertyName("name")]
  public required string Name { get; init; }

  [JsonPropertyName("address")]
  public required RandomUserAddress Address { get; init; }

  [JsonPropertyName("email")]
  public required string Email { get; init; }
}

public sealed record RandomUserAddress
{
  [JsonPropertyName("city")]
  public required string City { get; init; }
}
