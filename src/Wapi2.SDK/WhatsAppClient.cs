using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wapi2.SDK.Exceptions;
using Wapi2.SDK.Models;
using Wapi2.SDK.Utils;

namespace Wapi2.SDK
{
    public class WhatsAppClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly JsonSerializerSettings _jsonSettings;

        public WhatsAppClient(string token, string apiUrl = "https://wapi2.com")
        {
            if (string.IsNullOrEmpty(token))
                throw new ValidationException("El token de autenticación es requerido");

            _apiUrl = apiUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        #region Métodos de Autenticación
        public async Task<ApiResponse<Session>> CreateSessionAsync()
        {
            return await PostAsync<ApiResponse<Session>>("/auth/newsession");
        }

        public async Task<ApiResponse<Session>> CheckAuthAsync(string sessionId)
        {
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<Session>>($"/auth/checkauth/{sessionId}");
        }

        public async Task<ApiResponse<Session[]>> GetSessionsAsync()
        {
            return await GetAsync<ApiResponse<Session[]>>("/auth/getsessions");
        }

        public async Task<ApiResponse<object>> LogoutAsync(string sessionId)
        {
            ValidateSessionId(sessionId);
            return await PostAsync<ApiResponse<object>>($"/auth/logout/{sessionId}");
        }

        public async Task<ApiResponse<string>> GetQrCodeAsync(string sessionId)
        {
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<string>>($"/auth/getqr?session_id={sessionId}");
        }
        #endregion

        #region Métodos de Mensajería
        public async Task<ApiResponse<object>> SendMessageAsync(string to, string message, string sessionId)
        {
            if (string.IsNullOrEmpty(message))
                throw new ValidationException("El mensaje no puede estar vacío");

            ValidateRecipient(to);
            ValidateSessionId(sessionId);

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/message?session_id={sessionId}",
                new { message }
            );
        }

        public async Task<ApiResponse<object>> SendImageAsync(string to, string image, string caption, string sessionId)
        {
            ValidateRecipient(to);
            ValidateSessionId(sessionId);
            ValidateFileContent(image, "imagen");

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/image?session_id={sessionId}",
                new { image, caption }
            );
        }

        public async Task<ApiResponse<object>> SendPdfAsync(string to, string pdf, string filename, string caption, string sessionId)
        {
            ValidateRecipient(to);
            ValidateSessionId(sessionId);
            ValidateFileContent(pdf, "PDF");
            ValidateFilename(filename, ".pdf");

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/pdf?session_id={sessionId}",
                new { pdf, filename, caption }
            );
        }

        public async Task<ApiResponse<object>> SendVideoAsync(string to, string video, string caption, string sessionId)
        {
            ValidateRecipient(to);
            ValidateSessionId(sessionId);
            ValidateFileContent(video, "video");

            if (FileUtils.IsValidUrl(video))
            {
                string extension = Path.GetExtension(video).TrimStart('.').ToLower();
                if (!Constants.ALLOWED_VIDEO_FORMATS.Contains(extension))
                {
                    throw new FileValidationException(
                        $"Formato de video no válido. Formatos permitidos: {string.Join(", ", Constants.ALLOWED_VIDEO_FORMATS)}",
                        FileValidationException.FileErrorType.InvalidFormat
                    );
                }
            }

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/video?session_id={sessionId}",
                new { video, caption }
            );
        }

        public async Task<ApiResponse<object>> SendOfficeDocumentAsync(string to, string document, string filename, string caption, string sessionId)
        {
            ValidateRecipient(to);
            ValidateSessionId(sessionId);
            ValidateFileContent(document, "documento");
            ValidateOfficeFormat(filename);

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/office?session_id={sessionId}",
                new { document, filename, caption }
            );
        }

        public async Task<ApiResponse<object>> SendLocationAsync(string to, float latitude, float longitude, string description, string sessionId)
        {
            ValidateRecipient(to);
            ValidateSessionId(sessionId);

            if (latitude < -90 || latitude > 90)
                throw new ValidationException("Latitud debe estar entre -90 y 90");
            if (longitude < -180 || longitude > 180)
                throw new ValidationException("Longitud debe estar entre -180 y 180");

            return await PostAsync<ApiResponse<object>>(
                $"/chat/{to}/location?session_id={sessionId}",
                new { latitude, longitude, description }
            );
        }
        #endregion

        #region Métodos de Contactos
        public async Task<ApiResponse<Contact[]>> GetContactsAsync(string sessionId)
        {
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<Contact[]>>($"/contact/getcontacts?session_id={sessionId}");
        }

        public async Task<ApiResponse<Contact>> GetContactAsync(string phone, string sessionId)
        {
            ValidatePhone(phone);
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<Contact>>($"/contact/getcontact/{phone}?session_id={sessionId}");
        }

        public async Task<ApiResponse<string>> GetProfilePictureAsync(string phone, string sessionId)
        {
            ValidatePhone(phone);
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<string>>($"/contact/getprofilepic/{phone}?session_id={sessionId}");
        }

        public async Task<ApiResponse<bool>> IsRegisteredUserAsync(string phone, string sessionId)
        {
            ValidatePhone(phone);
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<bool>>($"/contact/isregistereduser/{phone}?session_id={sessionId}");
        }

        public async Task<ApiResponse<Group[]>> GetGroupsAsync(string sessionId)
        {
            ValidateSessionId(sessionId);
            return await GetAsync<ApiResponse<Group[]>>($"/contact/getgroups?session_id={sessionId}");
        }
        #endregion

        #region Métodos de Validación
        private void ValidateSessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ValidationException("El ID de sesión es requerido");
        }

        private void ValidateRecipient(string to)
        {
            if (string.IsNullOrEmpty(to))
                throw new ValidationException("El destinatario es requerido");
        }

        private void ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                throw new ValidationException("El número de teléfono es requerido");
        }

        private void ValidateFilename(string filename, string expectedExtension = null)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ValidationException("El nombre del archivo es requerido");

            if (filename.Length > Constants.MAX_FILENAME_LENGTH)
                throw new ValidationException($"El nombre del archivo excede {Constants.MAX_FILENAME_LENGTH} caracteres");

            if (expectedExtension != null && !filename.ToLower().EndsWith(expectedExtension))
                throw new FileValidationException(
                    $"El archivo debe tener la extensión {expectedExtension}",
                    FileValidationException.FileErrorType.InvalidFormat
                );
        }

        private void ValidateFileContent(string content, string fileType)
        {
            if (string.IsNullOrEmpty(content))
                throw new ValidationException($"El contenido del {fileType} es requerido");

            if (!FileUtils.IsValidUrl(content) && !FileUtils.IsBase64String(content))
                throw new FileValidationException(
                    $"El contenido del {fileType} debe ser una URL válida o una cadena Base64",
                    FileValidationException.FileErrorType.InvalidContent
                );

            if (FileUtils.IsBase64String(content))
            {
                var fileSize = FileUtils.GetBase64FileSize(content);
                if (fileSize > Constants.MAX_FILE_SIZE)
                    throw new FileValidationException(
                        $"El {fileType} excede el tamaño máximo de {Constants.MAX_FILE_SIZE / (1024 * 1024)}MB",
                        FileValidationException.FileErrorType.SizeExceeded
                    );
            }
        }

        private void ValidateOfficeFormat(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ValidationException("El nombre del archivo es requerido");

            string extension = Path.GetExtension(filename).TrimStart('.').ToLower();
            if (!Constants.ALLOWED_OFFICE_FORMATS.Contains(extension))
                throw new FileValidationException(
                    $"Formato de archivo no válido. Formatos permitidos: {string.Join(", ", Constants.ALLOWED_OFFICE_FORMATS)}",
                    FileValidationException.FileErrorType.InvalidFormat
                );
        }
        #endregion

        #region Métodos HTTP
        private async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl + endpoint);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex) when (!(ex is WhatsAppException))
            {
                throw new NetworkException($"Error al realizar la solicitud GET: {ex.Message}", ex);
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data = null)
        {
            try
            {
                StringContent content = null;
                if (data != null)
                {
                    var jsonData = JsonConvert.SerializeObject(data, _jsonSettings);
                    content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PostAsync(_apiUrl + endpoint, content);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex) when (!(ex is WhatsAppException))
            {
                throw new NetworkException($"Error al realizar la solicitud POST: {ex.Message}", ex);
            }
        }

        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            string content = null;
            try
            {
                content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    throw new WhatsAppException(
                        "El servidor no retornó ninguna respuesta",
                        WhatsAppException.ErrorType.Server,
                        500
                    );
                }

                if (!response.IsSuccessStatusCode)
                {
                    JObject errorData = null;
                    string errorMessage;

                    try
                    {
                        var errorObject = JObject.Parse(content);
                        errorMessage = errorObject["message"]?.ToString()
                            ?? errorObject["error"]?.ToString()
                            ?? "Error desconocido en la solicitud";
                        errorData = errorObject;
                    }
                    catch (JsonReaderException)
                    {
                        errorMessage = content;
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new ValidationException($"Error de validación: {errorMessage}", errorData);
                        case HttpStatusCode.Unauthorized:
                            throw new AuthenticationException($"Error de autenticación: {errorMessage}", errorData);
                        case HttpStatusCode.Forbidden:
                            throw new PermissionException($"Sin permisos: {errorMessage}", errorData);
                        case HttpStatusCode.NotFound:
                            throw new SessionNotFoundException($"Sesión no encontrada: {errorMessage}", errorData);
                        //case HttpStatusCode.TooManyRequests:
                        //    throw new RateLimitException($"Límite de solicitudes excedido: {errorMessage}", errorData);
                        default:
                            throw new WhatsAppException(
                                $"Error del servidor: {errorMessage}",
                                WhatsAppException.ErrorType.Server,
                                (int)response.StatusCode,
                                errorData
                            );
                    }
                }

                try
                {
                    return JsonConvert.DeserializeObject<T>(content, _jsonSettings);
                }
                catch (JsonException ex)
                {
                    throw new ValidationException(
                        $"Error al procesar la respuesta del servidor: {ex.Message}",
                        new JObject { ["content"] = content }
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                throw new NetworkException(
                    $"Error de conexión: {ex.Message}",
                    ex
                );
            }
            catch (TaskCanceledException)
            {
                throw new NetworkException("La solicitud excedió el tiempo de espera");
            }
            catch (Exception ex) when (!(ex is WhatsAppException))
            {
                System.Diagnostics.Debug.WriteLine($"Error inesperado: {ex}");
                System.Diagnostics.Debug.WriteLine($"Response content: {content}");

                throw new WhatsAppException(
                    $"Error inesperado: {ex.Message}",
                    WhatsAppException.ErrorType.Unknown,
                    500,
                    new JObject
                    {
                        ["error"] = ex.Message,
                        ["stackTrace"] = ex.StackTrace
                    }
                );
            }
        }
        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}