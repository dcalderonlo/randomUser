using RandomUser.src.Application.DTOs;
using RandomUser.src.Application.Interfaces;
using RandomUser.src.Domain.Entities;
using RandomUser.src.Domain.Exceptions;
using RandomUser.src.Infrastructure.Http;

namespace RandomUser.src.Infrastructure.Repositories;

// Implementación del repositorio de usuarios usando HTTP
// Adaptador entre la API externa y el dominio de la aplicación
public sealed class HttpUserRepository : IUserRepository, IDisposable
{
  private readonly RandomUserApiClient _apiClient;
  private readonly int _maxConcurrentRequests;

  public HttpUserRepository(ApiConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(configuration);
    _apiClient = new RandomUserApiClient(configuration);
    _maxConcurrentRequests = configuration.MaxConcurrentRequests;
  }

  // Obtiene un usuario aleatorio de la API
  public async Task<User> GetRandomUserAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      // Obtener la lista de usuarios de la API y mapear el primero a la entidad de dominio
      var users = await _apiClient.GetRandomUsersAsync(cancellationToken);
      return MapToEntity(users[0]);
    }
    catch (DomainException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new ConnectionException("Error inesperado al obtener usuario", ex);
    }
  }

  /// Obtiene múltiples usuarios aleatorios de la API con control de concurrencia
  public async Task<IReadOnlyList<User>> GetRandomUsersAsync(
    int count,
    IProgress<int>? progress = null,
    CancellationToken cancellationToken = default)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

    // Obtener la lista completa de usuarios de la API
    var apiUsers = await _apiClient.GetRandomUsersAsync(cancellationToken);
    if (apiUsers == null || apiUsers.Count == 0)
      throw new InvalidResponseException("La API no devolvió usuarios");

    // Mapear los usuarios obtenidos a la entidad de dominio con reporte de progreso
    var users = new List<User>();
    int max = Math.Min(count, apiUsers.Count);
    for (int i = 0; i < max; i++)
    {
      try
      {
        users.Add(MapToEntity(apiUsers[i]));
        progress?.Report(i + 1);
      }
      catch (Exception ex)
      {
        throw new ConnectionException($"Error al mapear usuario {i + 1}", ex);
      }
    }
    return users.AsReadOnly();
  }

  // Clase auxiliar para rastrear el progreso de obtención de usuarios
  private class ProgressTracker
  {
    private int _completed = 0;

    public int Completed => _completed;

  // Incrementa el contador de usuarios obtenidos
    public void Increment()
    {
      Interlocked.Increment(ref _completed);
    }
  }

  // Mapea el DTO de infraestructura a la entidad de dominio
  private static User MapToEntity(Http.DTOs.RandomUserResult result)
  {
    try
    {
      // Validar que los datos necesarios estén presentes en la respuesta
      if (string.IsNullOrWhiteSpace(result.Name) ||
          string.IsNullOrWhiteSpace(result.Email) ||
          string.IsNullOrWhiteSpace(result.Phone) ||
          result.Address is null ||
          string.IsNullOrWhiteSpace(result.Address.City))
      {
        throw new InvalidResponseException("Datos incompletos en la respuesta de la API");
      }

      // Crear y retornar la entidad de dominio User a partir del DTO
      return User.Create(
        fullName: result.Name,
        phone: result.Phone,
        email: result.Email,
        city: result.Address.City
      );
    }
    catch (Exception ex)
    {
      throw new InvalidResponseException("Error al mapear los datos del usuario", ex);
    }
  }

  // Liberar recursos del API client
  public void Dispose()
  {
    _apiClient?.Dispose();
    GC.SuppressFinalize(this);
  }
}
