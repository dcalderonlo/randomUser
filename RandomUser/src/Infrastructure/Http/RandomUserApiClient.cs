using System.Text.Json;
using RandomUser.src.Application.DTOs;
using RandomUser.src.Domain.Exceptions;
using RandomUser.src.Infrastructure.Http.DTOs;

namespace RandomUser.src.Infrastructure.Http;

// Cliente HTTP para la API de Random User
// Implementa la lógica de comunicación HTTP y reintentos
public sealed class RandomUserApiClient : IDisposable
{
  private readonly HttpClient _httpClient;
  private readonly ApiConfiguration _configuration;
  private readonly int _maxRetries;
  private const int DelayMilliseconds = 1000;

  public RandomUserApiClient(ApiConfiguration configuration)
  {
    // Validar que la configuración no sea nula
    ArgumentNullException.ThrowIfNull(configuration);
    _configuration = configuration;
    _maxRetries = configuration.MaxRetries;

    // Configurar HttpClient con la URL base y el timeout
    _httpClient = new HttpClient
    {
      Timeout = TimeSpan.FromSeconds(configuration.TimeoutSeconds)
    };
  }

  // Obtiene un usuario aleatorio de la API con reintentos automáticos
  public async Task<List<RandomUserResult>> GetRandomUsersAsync(CancellationToken cancellationToken = default)
  {
    return await ExecuteWithRetryAsync(async () =>
    {
      // Realizar la solicitud HTTP GET a la API
      var jsonResponse = await _httpClient.GetStringAsync(_configuration.BaseUrl, cancellationToken);
      return DeserializeResponse(jsonResponse);
    }, "Obtención de usuarios");
  }

  // Ejecuta una operación con reintentos automáticos
  private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName)
  {
    // Validar que la operación no sea nula
    ArgumentNullException.ThrowIfNull(operation);

    int attempts = 0;
    Exception? lastException = null;

    // Intentar ejecutar la operación hasta el máximo de reintentos
    while (attempts < _maxRetries)
    {
      try
      {
        attempts++;
        return await operation();
      }
      catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
      {
        lastException = ex;
        if (attempts >= _maxRetries)
        {
          throw new ConnectionException(
            $"Error de conexión después de {_maxRetries} intentos en {operationName}",
            ex);
        }

        // Esperar un tiempo antes de reintentar (backoff simple)
        await Task.Delay(DelayMilliseconds * attempts);
      }
    }

    throw new ConnectionException(
      $"No se pudo completar {operationName} después de {_maxRetries} intentos",
      lastException!);
  }

  // Deserializa la respuesta JSON de la API
  private static List<RandomUserResult> DeserializeResponse(string jsonResponse)
  {
    try
    {
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };

      // Deserializar la respuesta JSON a una lista de RandomUserResult
      var response = JsonSerializer.Deserialize<List<RandomUserResult>>(jsonResponse, options);
      if (response is null || response.Count == 0)
        throw new InvalidResponseException("La API no devolvió resultados válidos");
      return response;
    }
    catch (JsonException ex)
    {
      throw new InvalidResponseException("La respuesta de la API no tiene un formato válido", ex);
    }
  }

  // Liberar recursos del HttpClient
  public void Dispose()
  {
    _httpClient?.Dispose();
    GC.SuppressFinalize(this);
  }
}
