# 📱 Wapi2.SDK - SDK de WhatsApp para .NET

SDK oficial para integrar WhatsApp en tus aplicaciones .NET de manera rápida y sencilla. Envía mensajes, imágenes, documentos y más con pocas líneas de código.

## 🚀 Características

- ✉️ Envío de mensajes de texto
- 📸 Compartir imágenes y videos
- 📄 Envío de documentos (PDF, Office, ZIP)
- 📍 Compartir ubicación
- 👥 Gestión de contactos y grupos
- 🔒 Implementación segura y robusta
- ⚡ Alto rendimiento
- 🛡️ Manejo de errores inteligente

## 📦 Instalación

```bash
dotnet add package Wapi2.SDK
```

O a través del Package Manager de Visual Studio:
```powershell
Install-Package Wapi2.SDK
```

## 🔑 Requisitos Previos

1. Regístrate en [wapi2.com](https://wapi2.com)
2. Obtén tu token de acceso
3. Crea una sesión y escanea el código QR con tu WhatsApp
4. ¡Listo para empezar!

## 📝 Ejemplos de Uso

### Configuración Inicial
```csharp
using Wapi2.SDK;

// Inicializar cliente con tu token
var client = new WhatsAppClient("TU-TOKEN-AQUI");
```

### 💬 Enviar Mensaje de Texto
```csharp
try
{
    var response = await client.SendMessageAsync(
        to: "51999999999",  // Número en formato internacional
        message: "¡Hola desde .NET!",
        sessionId: "TU-SESSION-ID"
    );

    if (response.Status == "success")
        Console.WriteLine("Mensaje enviado exitosamente");
}
catch (WhatsAppException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### 🖼️ Enviar Imagen
```csharp
// Desde URL
await client.SendImageAsync(
    to: "51999999999",
    image: "https://ejemplo.com/imagen.jpg",
    caption: "¡Mira esta imagen!",
    sessionId: "TU-SESSION-ID"
);

// Desde archivo local
string base64Image = Convert.ToBase64String(File.ReadAllBytes("ruta/imagen.jpg"));
await client.SendImageAsync(
    to: "51999999999",
    image: base64Image,
    caption: "Imagen desde archivo local",
    sessionId: "TU-SESSION-ID"
);
```

### 📹 Enviar Video
```csharp
// Admite formatos: mp4, 3gp, mov (máx 16MB)
await client.SendVideoAsync(
    to: "51999999999",
    video: "https://ejemplo.com/video.mp4",
    caption: "¡Mira este video!",
    sessionId: "TU-SESSION-ID"
);
```

### 📑 Enviar PDF
```csharp
await client.SendPdfAsync(
    to: "51999999999",
    pdf: "https://ejemplo.com/documento.pdf",
    filename: "documento.pdf",
    caption: "Documento importante",
    sessionId: "TU-SESSION-ID"
);
```

### 📊 Enviar Documento de Office
```csharp
// Soporta: doc, docx, xls, xlsx, ppt, pptx
await client.SendOfficeDocumentAsync(
    to: "51999999999",
    document: "https://ejemplo.com/reporte.xlsx",
    filename: "reporte.xlsx",
    caption: "Reporte mensual",
    sessionId: "TU-SESSION-ID"
);
```

### 📦 Enviar Archivo ZIP
```csharp
await client.SendFileAsync(
    to: "51999999999",
    document: "https://ejemplo.com/archivos.zip",
    filename: "archivos.zip",
    caption: "Documentos comprimidos",
    sessionId: "TU-SESSION-ID"
);
```

### 📍 Enviar Ubicación
```csharp
await client.SendLocationAsync(
    to: "51999999999",
    latitude: -12.0464,
    longitude: -77.0428,
    description: "Plaza Mayor de Lima",
    sessionId: "TU-SESSION-ID"
);
```

## ⚙️ Configuración Avanzada

### Manejo de Errores
```csharp
try
{
    await client.SendMessageAsync(to, message, sessionId);
}
catch (ValidationException ex)
{
    Console.WriteLine($"Error de validación: {ex.Message}");
}
catch (FileValidationException ex) when (ex.FileError == FileValidationException.FileErrorType.SizeExceeded)
{
    Console.WriteLine("El archivo excede el tamaño máximo de 16MB");
}
catch (NetworkException ex)
{
    Console.WriteLine($"Error de red: {ex.Message}");
}
catch (WhatsAppException ex)
{
    Console.WriteLine($"Error general: {ex.Message}");
}
```

### Límites y Restricciones
- Tamaño máximo de archivos: 16MB
- Longitud máxima del nombre de archivo: 255 caracteres
- Longitud máxima de descripción: 1024 caracteres
- Formatos de video soportados: mp4, 3gp, mov
- Formatos de Office soportados: doc, docx, xls, xlsx, ppt, pptx

## 🛠️ Plataformas Soportadas

- .NET Framework 4.6.1 o superior
- Cualquier plataforma que soporte .NET Standard 1.6, incluyendo:
  - .NET Core 1.0 o superior
  - Xamarin.iOS 10.0 o superior
  - Xamarin.Android 7.0 o superior
  - Universal Windows Platform 10.0 o superior
  - Unity 2018.1 o superior

## 📘 Documentación
Para más detalles y ejemplos, visita nuestra [documentación completa](https://wapi2.com/api-docs).

## 🤝 Contribuir
¡Las contribuciones son bienvenidas! Por favor, lee nuestra [guía de contribución](CONTRIBUTING.md).

## 📄 Licencia
Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 🆘 Soporte
- 📧 Email: admin@wapi2.com
- 💬 Chat: [+51924893117](https://wa.me/51924893117)
- 📚 [Documentación API](https://wapi2.com/api-docs)
- 🐛 [Reportar Issues](https://github.com/wapi2/sdk-dotnet/issues)