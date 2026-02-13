using System.Text.Json.Serialization;

namespace RandomUser.src.Infrastructure.Http.DTOs;

// DTOs para deserializar la respuesta de la API externa
public sealed record RandomUserApiResponse
{
  [JsonPropertyName("results")]
  public required List<RandomUserResult> Results { get; init; }
}

// DTOs anidados para mapear la estructura de la respuesta JSON
// Solo incluye los campos que nos interesan para el funcionamiento del dominio
public sealed record RandomUserResult
{
  [JsonPropertyName("gender")]
  public required string Gender { get; init; }

  [JsonPropertyName("name")]
  public required RandomUserName Name { get; init; }

  [JsonPropertyName("location")]
  public required RandomUserLocation Location { get; init; }

  [JsonPropertyName("email")]
  public required string Email { get; init; }
}

public sealed record RandomUserName
{
  [JsonPropertyName("first")]
  public required string First { get; init; }

  [JsonPropertyName("last")]
  public required string Last { get; init; }
}

public sealed record RandomUserLocation
{
  [JsonPropertyName("country")]
  public required string Country { get; init; }
}
