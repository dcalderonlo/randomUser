namespace RandomUser.src.Domain.Entities;

// Entidad de dominio: Usuario
// Representa un usuario en el sistema sin dependencias externas
public sealed class User
{
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Gender { get; init; }
  public required string Email { get; init; }
  public required string Country { get; init; }

  public string FullName => $"{FirstName} {LastName}";

  private User() { }

  public static User Create(string firstName, string lastName, string gender, string email, string country)
  {
    // Validar datos de entrada
    ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
    ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));
    ArgumentException.ThrowIfNullOrWhiteSpace(gender, nameof(gender));
    ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
    ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));

    return new User
    {
      FirstName = firstName,
      LastName = lastName,
      Gender = gender,
      Email = email,
      Country = country
    };
  }

  public override string ToString() =>
    $"""
    Nombre: {FullName}
    Género: {Gender}
    Email: {Email}
    País: {Country}
    """;
}