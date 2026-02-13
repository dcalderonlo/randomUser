using Microsoft.Extensions.Configuration;
using RandomUser.src.Application.DTOs;
using RandomUser.src.Domain.Exceptions;

namespace RandomUser.src.Infrastructure.Configuration;

// Servicio de configuración - Lee y valida la configuración desde appsettings.json
public sealed class ConfigurationProvider
{
  private const string SectionName = "ApiSettings";
  private readonly IConfiguration _configuration;

  /// Constructor - Carga la configuración al crear la instancia
  public ConfigurationProvider()
  {
    _configuration = BuildConfiguration();
  }

  // Obtiene la configuración de la API validada
  public ApiConfiguration GetApiConfiguration()
  {
    // Validar que se pudo cargar la configuración
    var config = _configuration
      .GetSection(SectionName)
      .Get<ApiConfiguration>() ?? throw new InvalidConfigurationException(
        $"No se pudo cargar la configuración de '{SectionName}'. Verifica que appsettings.json existe y es válido.");

    // Validar la configuración
    try
    {
      config.Validate();
    }
    catch (Exception ex)
    {
      throw new InvalidConfigurationException($"Configuración inválida: {ex.Message}");
    }

    return config;
  }

  // Construye la configuración desde archivos JSON
  private static IConfiguration BuildConfiguration()
  {
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

    return new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
      .Build();
  }
}
