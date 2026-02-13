using RandomUser.src.Application.Interfaces;
using RandomUser.src.Domain.Entities;

namespace RandomUser.src.Application.Services;

// Caso de Uso: Obtener Usuarios Aleatorios
// Contiene la lógica de negocio de la aplicación
// No depende de detalles de implementación (HTTP, consola, etc.)
public sealed class GetRandomUsersUseCase(IUserRepository userRepository, IUserPresenter presenter)
{
  private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
  private readonly IUserPresenter _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

  // Ejecuta el caso de uso para obtener usuarios aleatorios
  public async Task<IReadOnlyList<User>> ExecuteAsync(int count, CancellationToken cancellationToken = default)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count, nameof(count));

    // Progreso de obtención de usuarios
    var progress = new Progress<int>(current =>
    {
        _presenter.DisplayProgress(current, count);
    });

    // Obtener usuarios del repositorio
    var users = await _userRepository.GetRandomUsersAsync(count, progress, cancellationToken);

    return users;
  }

  /// Presenta los resultados al usuario
  public void PresentResults(IReadOnlyList<User> users)
  {
    if (users.Count == 0)
    {
        _presenter.DisplayWarning("No se pudo obtener ningún usuario.");
        return;
    }

    for (int i = 0; i < users.Count; i++)
    {
        _presenter.DisplayUser(users[i], i + 1, users.Count);
    }

    _presenter.DisplaySuccess($"Se obtuvieron {users.Count} usuario(s) exitosamente.");
  }
}
