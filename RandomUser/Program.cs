using RandomUser.src.Application.Interfaces;
using RandomUser.src.Application.Services;
using RandomUser.src.Domain.Exceptions;
using RandomUser.src.Infrastructure.Configuration;
using RandomUser.src.Infrastructure.Repositories;
using RandomUser.src.Presentation.Console;

namespace RandomUser.src;

// Punto de entrada de la aplicación - Composition Root
// Responsable de:
// 1. Cargar la configuración
// 2. Componer las dependencias (Dependency Injection manual)
// 3. Iniciar la aplicación
public static class Program
{
  static async Task Main(string[] args)
  {
    try
    {
      // Configurar consola
      System.Console.OutputEncoding = System.Text.Encoding.UTF8;

      // Cargar configuración
      var configProvider = new ConfigurationProvider();
      var apiConfig = configProvider.GetApiConfiguration();

      // Mostrar configuración en desarrollo
      DisplayConfigurationIfDevelopment(apiConfig);

      // Composición de dependencias (Dependency Injection manual)
      // CAPA DE INFRAESTRUCTURA
      using IUserRepository userRepository = new HttpUserRepository(apiConfig);

      // CAPA DE PRESENTACIÓN
      IUserPresenter presenter = new ConsoleUserPresenter();

      // CAPA DE APLICACIÓN (Casos de Uso)
      var getUsersUseCase = new GetRandomUsersUseCase(userRepository, presenter);

      // CONTROLADOR DE PRESENTACIÓN
      var controller = new ConsoleApplicationController(getUsersUseCase, presenter);

      // Ejecutar aplicación
      using var cts = new CancellationTokenSource();
      System.Console.CancelKeyPress += (_, e) =>
      {
        e.Cancel = true;
        cts.Cancel();
      };

      await controller.RunAsync(cts.Token);
    }
    catch (InvalidConfigurationException ex)
    {
      System.Console.ForegroundColor = ConsoleColor.Red;
      System.Console.WriteLine($"\n❌ Error de configuración: {ex.Message}");
      System.Console.WriteLine("\nVerifica que el archivo 'appsettings.json' existe y es válido.");
      System.Console.ResetColor();
      Environment.Exit(1);
    }
    catch (Exception ex)
    {
      System.Console.ForegroundColor = ConsoleColor.Red;
      System.Console.WriteLine($"\n❌ Error fatal: {ex.Message}");
      System.Console.ResetColor();
      Environment.Exit(1);
    }
  }

  private static void DisplayConfigurationIfDevelopment(Application.DTOs.ApiConfiguration config)
  {
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

    if (environment == "Development")
    {
      System.Console.ForegroundColor = ConsoleColor.DarkGray;
      System.Console.WriteLine("╔══════════════════════════════════════════╗");
      System.Console.WriteLine("║    Configuración Cargada (Desarrollo)    ║");
      System.Console.WriteLine("╚══════════════════════════════════════════╝");
      System.Console.WriteLine($"API Base URL: {config.BaseUrl}");
      System.Console.WriteLine($"Timeout: {config.TimeoutSeconds}s");
      System.Console.WriteLine($"Max Reintentos: {config.MaxRetries}");
      System.Console.WriteLine($"Solicitudes Concurrentes: {config.MaxConcurrentRequests}");
      System.Console.WriteLine();
      System.Console.ResetColor();
    }
  }
}
