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
      // Obtener un usuario de la API y mapearlo a la entidad de dominio
      var response = await _apiClient.GetRandomUserAsync(cancellationToken);
      return MapToEntity(response.Results[0]);
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

    var users = new List<User>();
    var tasks = new List<Task<User>>();
    var progressTracker = new ProgressTracker();

    for (int i = 0; i < count; i++)
    {
      var task = GetRandomUserWithProgressAsync(progress, progressTracker, count, cancellationToken);
      tasks.Add(task);

      // Limitar la concurrencia
      if (tasks.Count >= _maxConcurrentRequests)
      {
        var completedTask = await Task.WhenAny(tasks);
        tasks.Remove(completedTask);

        try
        {
          users.Add(await completedTask);
        }
        catch (Exception ex)
        {
          throw new ConnectionException("Error al obtener usuarios", ex);
        }
      }
    }

    // Esperar a que terminen todas las tareas restantes
    var results = await Task.WhenAll(tasks);
    // Agregar los resultados exitosos a la lista de usuarios
    users.AddRange(results.Where(u => u is not null));

    return users.AsReadOnly();
  }

  // Obtiene un usuario con reporte de progreso
  private async Task<User> GetRandomUserWithProgressAsync(
    IProgress<int>? progress, 
    ProgressTracker progressTracker, 
    int total,
    CancellationToken cancellationToken)
  {
    try
    {
      // Obtener un usuario de la API y mapearlo a la entidad de dominio
      var user = await GetRandomUserAsync(cancellationToken);
      progressTracker.Increment();
      progress?.Report(progressTracker.Completed);
      return user;
    }
    catch (DomainException ex)
    {
      // Si ocurre un error de dominio, se considera como un usuario no obtenido
      progressTracker.Increment();
      progress?.Report(progressTracker.Completed);
      throw new ConnectionException($"Error al obtener usuario", ex);
    }
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
      if (string.IsNullOrWhiteSpace(result.Name.First) ||
          string.IsNullOrWhiteSpace(result.Name.Last) ||
          string.IsNullOrWhiteSpace(result.Email) ||
          string.IsNullOrWhiteSpace(result.Location.Country))
      {
        throw new InvalidResponseException("Datos incompletos en la respuesta de la API");
      }

      // Crear y retornar la entidad de dominio User a partir del DTO
      return User.Create(
        firstName: result.Name.First,
        lastName: result.Name.Last,
        gender: result.Gender,
        email: result.Email,
        country: result.Location.Country
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
