namespace RandomUser.src.Application.DTOs;

// DTO: Configuración de la API
// Objeto de transferencia de datos entre capas
public sealed record ApiConfiguration
{
  public required string BaseUrl { get; init; }
  public required int TimeoutSeconds { get; init; }
  public required int MaxRetries { get; init; }
  public required int MaxConcurrentRequests { get; init; }

  public void Validate()
  {
    if (string.IsNullOrWhiteSpace(BaseUrl))
      throw new ArgumentException("BaseUrl no puede estar vacío", nameof(BaseUrl));

    if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
      throw new ArgumentException("BaseUrl debe ser una URL válida", nameof(BaseUrl));

    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(TimeoutSeconds, nameof(TimeoutSeconds));
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxRetries, nameof(MaxRetries));
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxConcurrentRequests, nameof(MaxConcurrentRequests));
  }
}