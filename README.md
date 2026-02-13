# RandomUser

## Descripción

RandomUser es una aplicación de consola desarrollada en C# que demuestra el uso de Clean Architecture, principios SOLID y buenas prácticas de desarrollo. El propósito del proyecto es obtener y mostrar información de usuarios desde una API externa (actualmente utilizando JSONPlaceholder) de manera estructurada y mantenible.

La aplicación permite obtener uno o múltiples usuarios aleatorios, mostrando sus datos como nombre completo, teléfono, email y ciudad.

## Características

- Arquitectura limpia (Clean Architecture) con separación de capas: Domain, Application, Infrastructure y Presentation.
- Implementación de principios SOLID.
- Manejo de errores robusto y validaciones.
- Soporte para obtener múltiples usuarios con reporte de progreso.
- Configuración flexible a través de archivos JSON.

## Requisitos

- .NET 8.0 o superior
- SDK de .NET instalado

## Instalación

1. Clona el repositorio:
   ```bash
   git clone https://github.com/tu-usuario/randomuser.git
   cd randomuser
   ```

2. Restaura los paquetes NuGet:
   ```bash
   dotnet restore
   ```

## Uso

1. Compila el proyecto:
   ```bash
   dotnet build
   ```

2. Ejecuta la aplicación:
   ```bash
   dotnet run --project RandomUser/RandomUser.csproj
   ```

3. Sigue las instrucciones en consola para especificar cuántos usuarios deseas obtener.

## Arquitectura

El proyecto sigue los principios de Clean Architecture:

- **Domain**: Contiene las entidades de negocio y reglas de dominio.
- **Application**: Define los casos de uso y interfaces.
- **Infrastructure**: Implementa los detalles técnicos (repositorios, configuración, API clients).
- **Presentation**: Maneja la interfaz de usuario (consola en este caso).

## Tecnologías Utilizadas

- C# 12
- .NET 8.0
- System.Text.Json para deserialización
- HttpClient para llamadas a API

## Configuración

La configuración se encuentra en `appsettings.json` y `appsettings.Development.json`. Puedes modificar la URL de la API, timeouts, reintentos, etc.

## Contribución

Si deseas contribuir:
1. Haz un fork del proyecto.
2. Crea una rama para tu feature.
3. Envía un pull request.

## Licencia

Este proyecto es de uso educativo y no tiene licencia específica.

## Autor

Desarrollado como parte de la Tarea Práctica 3 - Unidad 3.