using RandomUser.src.Application.Interfaces;
using RandomUser.src.Domain.Entities;

namespace RandomUser.src.Presentation.Console;

// Presentador de consola - Implementa la interfaz de presentación
// Maneja toda la lógica de visualización en consola
public sealed class ConsoleUserPresenter : IUserPresenter
{
  private readonly Lock _lock = new();

  // Muestra un usuario en consola con formato amigable
  public void DisplayUser(User user, int index, int total)
  {
    lock (_lock)
    {
      System.Console.ForegroundColor = ConsoleColor.Cyan;
      System.Console.WriteLine($"\n{'═'} Usuario {index} de {total} {'═'}");
      System.Console.ResetColor();

      System.Console.ForegroundColor = ConsoleColor.White;
      System.Console.WriteLine(user.ToString());
      System.Console.ResetColor();
    }
  }

  // Muestra el progreso de la carga de usuarios en consola
  public void DisplayProgress(int current, int total)
  {
    lock (_lock)
    {
      var percentage = current * 100 / total;
      var progressBar = GenerateProgressBar(percentage);

      System.Console.SetCursorPosition(0, System.Console.CursorTop);
      System.Console.ForegroundColor = ConsoleColor.Yellow;
      System.Console.Write($"Progreso: [{progressBar}] {current}/{total} ({percentage}%)    ");
      System.Console.ResetColor();
    }
  }

  // Muestra un mensaje de error en consola
  public void DisplayError(string message)
  {
    lock (_lock)
    {
      System.Console.ForegroundColor = ConsoleColor.Red;
      System.Console.WriteLine($"\n❌ Error: {message}");
      System.Console.ResetColor();
    }
  }

  // Muestra un mensaje de éxito en consola
  public void DisplaySuccess(string message)
  {
    lock (_lock)
    {
      System.Console.ForegroundColor = ConsoleColor.Green;
      System.Console.WriteLine($"\n✓ {message}");
      System.Console.ResetColor();
    }
  }

  // Muestra un mensaje de advertencia en consola
  public void DisplayWarning(string message)
  {
    lock (_lock)
    {
      System.Console.ForegroundColor = ConsoleColor.Yellow;
      System.Console.WriteLine($"\n⚠ {message}");
      System.Console.ResetColor();
    }
  }

  // Genera una barra de progreso visual para mostrar el avance
  private static string GenerateProgressBar(int percentage)
  {
    const int barLength = 30;
    var filled = (percentage * barLength) / 100;
    var empty = barLength - filled;

    return new string('█', filled) + new string('░', empty);
  }
}
