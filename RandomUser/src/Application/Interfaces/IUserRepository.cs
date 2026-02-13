using RandomUser.src.Domain.Entities;

namespace RandomUser.src.Application.Interfaces
{
  // Repositorio de usuarios - Define el contrato para obtener usuarios
  // Esta interfaz pertenece a la capa de aplicación pero será implementada en infraestructura
  public interface IUserRepository: IDisposable
  {
    // Obtiene un usuario aleatorio
    Task<User> GetRandomUserAsync(CancellationToken cancellationToken = default);

    /// Obtiene múltiples usuarios aleatorios
    Task<IReadOnlyList<User>> GetRandomUsersAsync(int count, IProgress<int>? progress = null, CancellationToken cancellationToken = default);
  }
}
