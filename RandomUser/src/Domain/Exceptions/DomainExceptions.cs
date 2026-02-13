namespace RandomUser.src.Domain.Exceptions;

// Excepción base del dominio
public abstract class DomainException(string message) : Exception(message)
{
  public DomainException(string message, Exception innerException) : this(message)
  {
  }
}

// Excepción cuando hay problemas de conectividad
public sealed class ConnectionException(string message) : DomainException(message)
{
  public ConnectionException(string message, Exception innerException) : this(message)
  {
  }
}

// Excepción cuando la respuesta es inválida
public sealed class InvalidResponseException(string message) : DomainException(message)
{
  public InvalidResponseException(string message, Exception innerException) : this(message)
  {
  }
}

/// Excepción cuando la configuración es inválida
public sealed class InvalidConfigurationException(string message) : DomainException(message);
