using RandomUser.src.Application.Interfaces;
using RandomUser.src.Application.Services;
using RandomUser.src.Domain.Exceptions;

namespace RandomUser.src.Presentation.Console;

// Controlador de la aplicación de consola
// Orquesta el flujo de la aplicación y maneja la interacción con el usuario
public sealed class ConsoleApplicationController(GetRandomUsersUseCase useCase, IUserPresenter presenter)
{
  private readonly GetRandomUsersUseCase _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
  private readonly IUserPresenter _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

  // Ejecuta el flujo principal de la aplicación
  public async Task RunAsync(CancellationToken cancellationToken = default)
  {
    bool continueRunning = true;

    DisplayWelcomeMessage();

    // Bucle principal de la aplicación - Permite al usuario realizar múltiples consultas hasta que decida salir
    while (continueRunning && !cancellationToken.IsCancellationRequested)
    {
      try
      {
        var userCount = GetUserCountFromInput();

        if (userCount > 0)
        {
          await FetchAndDisplayUsersAsync(userCount, cancellationToken);
          continueRunning = AskToContinue();
        }
        else
        {
          continueRunning = false;
        }
      }
      catch (ConnectionException ex)
      {
        _presenter.DisplayError($"Problema de conexión: {ex.Message}");
        continueRunning = AskToRetry();
      }
      catch (InvalidResponseException ex)
      {
        _presenter.DisplayError($"Respuesta inválida de la API: {ex.Message}");
        continueRunning = AskToRetry();
      }
      catch (DomainException ex)
      {
        _presenter.DisplayError($"Error: {ex.Message}");
        continueRunning = AskToRetry();
      }
      catch (OperationCanceledException)
      {
        _presenter.DisplayWarning("Operación cancelada por el usuario.");
        continueRunning = false;
      }
      catch (Exception ex)
      {
        _presenter.DisplayError($"Error inesperado: {ex.Message}");
        continueRunning = false;
      }
    }

    DisplayGoodbyeMessage();
  }

  // Obtiene y muestra los usuarios
  private async Task FetchAndDisplayUsersAsync(int count, CancellationToken cancellationToken)
  {
    System.Console.WriteLine($"\n🔄 Obteniendo {count} usuario(s) aleatorio(s)...\n");

    try
    {
      var users = await _useCase.ExecuteAsync(count, cancellationToken);
      System.Console.WriteLine("\n");
      _useCase.PresentResults(users);
    }
    catch (Exception)
    {
      System.Console.WriteLine();
      throw;
    }
  }

  // Solicita al usuario cuántos usuarios desea obtener
  private int GetUserCountFromInput()
  {
      while (true)
      {
        System.Console.Write("\n¿Cuántos usuarios aleatorios deseas obtener? (0 para salir): ");
        var input = System.Console.ReadLine();

        if (int.TryParse(input, out int count))
        {
          return count switch
          {
            0 => 0,
            > 0 and <= 100 => count,
            _ => ShowWarningAndContinue("Por favor ingresa un número entre 1 y 100.")
          };
        }

        ShowWarningAndContinue("Por favor ingresa un número válido.");
      }

      // Muestra una advertencia y continúa el bucle para pedir una nueva entrada
      int ShowWarningAndContinue(string message)
      {
        _presenter.DisplayWarning(message);
        return -1;
      }
  }

  // Pregunta al usuario si desea continuar realizando consultas
  private static bool AskToContinue()
  {
    System.Console.Write("\n¿Deseas buscar más usuarios? (S/N): ");
    var response = System.Console.ReadLine()?.Trim().ToUpperInvariant();
    return response is "S" or "SI" or "SÍ" or "Y" or "YES";
  }

  // Pregunta al usuario si desea intentar nuevamente después de un error
  private static bool AskToRetry()
  {
      System.Console.Write("\n¿Deseas intentar nuevamente? (S/N): ");
      var response = System.Console.ReadLine()?.Trim().ToUpperInvariant();
      return response is "S" or "SI" or "SÍ" or "Y" or "YES";
  }

  // Muestra un mensaje de bienvenida al iniciar la aplicación
  private static void DisplayWelcomeMessage()
  {
      System.Console.Clear();
      System.Console.ForegroundColor = ConsoleColor.Green;
      System.Console.WriteLine("""
          ╔════════════════════════════════════════════════════════╗
          ║                                                        ║
          ║       🎲  GENERADOR DE USUARIOS ALEATORIOS  🎲         ║
          ║                                                        ║
          ╚════════════════════════════════════════════════════════╝
          """);
      System.Console.ResetColor();
  }

    // Muestra un mensaje de despedida al finalizar la aplicación
  private static void DisplayGoodbyeMessage()
  {
      System.Console.ForegroundColor = ConsoleColor.Green;
      System.Console.WriteLine("""
          
          ¡Gracias por usar el generador de usuarios aleatorios!
          👋 ¡Hasta pronto!
          
          """);
      System.Console.ResetColor();
  }
}
