using System;
using System.IO;
using System.Threading.Tasks;
using Wapi2.SDK;
using Wapi2.SDK.Exceptions;
using Wapi2.SDK.Models;

namespace Wapi2.SDK.ConsoleTest
{
    class Program
    {
        private static readonly string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoyLCJ0eXBlIjoiYXBpX2FjY2VzcyIsImlhdCI6MTczNzEzMTM3NywiZXhwIjoxNzY4NjY3Mzc3fQ.XAgaBYtl9Np6dkmxsFzzxRpOzYCSzydypiu8Mtouq4s";
        private static WhatsAppClient client;

        static void Main(string[] args)
        {
            try
            {
                /* - SESIONES -
                 * Las sesiones son creadas en wapi2.com, regístrate e inicia sesión.
                 * Ve a sesiones, crea una sesión y escanea el QR con el whatsapp que vas a interactuar
                 * Copia el id de sesión y utiliza el SDK
                 */
                string sessionId = "7cf14747-4c74-4f08-8f06-1ecb10819a16";
                client = new WhatsAppClient(Token);
                RunTests(sessionId).Wait();
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aggEx && aggEx.InnerException != null)
                {
                    ex = aggEx.InnerException;
                }

                Console.WriteLine($"Error: {ex.Message}");
                if (ex is WhatsAppException waEx && waEx.ErrorData != null)
                {
                    Console.WriteLine($"Error Data: {waEx.ErrorData}");
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        static async Task RunTests(string sessionId)
        {

            // 1. Prueba de mensaje de texto
            Console.WriteLine("\nEnviando mensaje de texto...");
            var messageResponse = await client.SendMessageAsync(
                "51946357405",  // Ejemplo: "51999999999"
                "Hola mundo!",
                sessionId
            );
            Console.WriteLine($"Mensaje enviado: {messageResponse.Status}");
            
            // 5. Prueba de imagen desde archivo
            Console.WriteLine("\n¿Desea probar el envío de imagen desde archivo? (S/N)");
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                Console.WriteLine("Ingrese la ruta completa del archivo de imagen:");
                string imagePath = Console.ReadLine();

                if (File.Exists(imagePath))
                {
                    string base64Image = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                    var imageResponse = await client.SendImageAsync(
                        "NUMERO-DESTINO",
                        base64Image,
                        "Prueba de imagen desde .NET SDK",
                        sessionId
                    );
                    Console.WriteLine($"Imagen enviada: {imageResponse.Status}");
                }
                else
                {
                    Console.WriteLine("Archivo no encontrado.");
                }
            }

            // 6. Prueba de imagen desde URL
            Console.WriteLine("\n¿Desea probar el envío de imagen desde URL? (S/N)");
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                Console.WriteLine("Ingrese la URL de la imagen:");
                string imageUrl = Console.ReadLine();

                var urlImageResponse = await client.SendImageAsync(
                    "51946357405",
                    imageUrl,
                    "Bien especialista!",
                    sessionId
                );
                Console.WriteLine($"Imagen enviada: {urlImageResponse.Status}");
            }
            

            // 7. Obtener contactos
            Console.WriteLine("\nObteniendo contactos...");
            var contacts = await client.GetContactsAsync(sessionId);
            if (contacts.Data != null)
            {
                foreach (var contact in contacts.Data)
                {
                    Console.WriteLine($"Contacto: {contact.Name} - {contact.Number}");
                }
            }

            // 8. Obtener grupos
            Console.WriteLine("\nObteniendo grupos...");
            var groups = await client.GetGroupsAsync(sessionId);
            if (groups.Data != null)
            {
                foreach (var group in groups.Data)
                {
                    Console.WriteLine($"Grupo: {group.Name} (Admin: {group.IsAdmin})");
                }
            }

            Console.WriteLine("\nTodas las pruebas completadas exitosamente!");
        }
    }
}