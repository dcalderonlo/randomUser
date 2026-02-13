namespace RandomUser.src.Domain.Entities;

// Entidad de dominio: Usuario
// Representa un usuario en el sistema sin dependencias externas
public sealed class User
{
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Phone { get; init; }
  public required string Email { get; init; }
  public required string City { get; init; }

  public string FullName => $"{FirstName} {LastName}";

  private User() { }

  public static User Create(string fullName, string phone, string email, string city)
  {
    // Validar datos de entrada
    ArgumentException.ThrowIfNullOrWhiteSpace(fullName, nameof(fullName));
    ArgumentException.ThrowIfNullOrWhiteSpace(phone, nameof(phone));
    ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
    ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));

    // Como la nueva API solo tiene el nombre completo, asignamos todo a FirstName y dejamos LastName vacío
    return new User
    {
      FirstName = fullName,
      LastName = string.Empty,
      Phone = phone,
      Email = email,
      City = city
    };
  }

  public override string ToString() =>
    $"""
    Nombre: {FullName}
    Teléfono: {Phone}
    Email: {Email}
    Ciudad: {City}
    """;
}
