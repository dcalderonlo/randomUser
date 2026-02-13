using RandomUser.src.Domain.Entities;

namespace RandomUser.src.Application.Interfaces;

// Presentador de usuarios - Esta interfaz define cómo se presentan los datos al usuario
// Esta interfaz pertenece a la capa de aplicación pero será implementada en Presentation
public interface IUserPresenter
{
  void DisplayUser(User user, int index, int total);
  void DisplayProgress(int current, int total);
  void DisplayError(string message);
  void DisplaySuccess(string message);
  void DisplayWarning(string message);
}
