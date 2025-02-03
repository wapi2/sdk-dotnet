using System;
using Newtonsoft.Json.Linq;

namespace Wapi2.SDK.Exceptions
{
    public class WhatsAppException : Exception
    {
        public int StatusCode { get; }
        public JObject ErrorData { get; }
        public ErrorType Type { get; }

        public enum ErrorType
        {
            Unknown,
            Validation,
            Authentication,
            Session,
            Network,
            Server,
            RateLimit
        }

        public WhatsAppException(string message, ErrorType type, int statusCode = 0, JObject errorData = null)
            : base(message)
        {
            Type = type;
            StatusCode = statusCode;
            ErrorData = errorData;
        }

        public static WhatsAppException Create(string message, int statusCode, JObject errorData = null)
        {
            // Determinar el tipo de error basado en el código de estado HTTP
            switch (statusCode)
            {
                case 400:
                    return new ValidationException(message, errorData);
                case 401:
                    return new AuthenticationException(message, errorData);
                case 403:
                    return new PermissionException(message, errorData);
                case 404:
                    return new SessionNotFoundException(message, errorData);
                case 429:
                    return new RateLimitException(message, errorData);
                case 500:
                case 501:
                case 502:
                case 503:
                case 504:
                    return new ServerException(message, statusCode, errorData);
                default:
                    return new WhatsAppException(message, ErrorType.Unknown, statusCode, errorData);
            }
        }
    }

    public class ValidationException : WhatsAppException
    {
        public ValidationException(string message, JObject errorData = null)
            : base(message, ErrorType.Validation, 400, errorData)
        {
        }
    }

    public class AuthenticationException : WhatsAppException
    {
        public AuthenticationException(string message, JObject errorData = null)
            : base(message, ErrorType.Authentication, 401, errorData)
        {
        }
    }

    public class PermissionException : WhatsAppException
    {
        public PermissionException(string message, JObject errorData = null)
            : base(message, ErrorType.Authentication, 403, errorData)
        {
        }
    }

    public class SessionNotFoundException : WhatsAppException
    {
        public SessionNotFoundException(string message, JObject errorData = null)
            : base(message, ErrorType.Session, 404, errorData)
        {
        }
    }

    public class RateLimitException : WhatsAppException
    {
        public int? RetryAfterSeconds { get; }

        public RateLimitException(string message, JObject errorData = null)
            : base(message, ErrorType.RateLimit, 429, errorData)
        {
            if (errorData?["retry_after"] != null)
            {
                RetryAfterSeconds = errorData["retry_after"].Value<int>();
            }
        }
    }

    public class ServerException : WhatsAppException
    {
        public ServerException(string message, int statusCode, JObject errorData = null)
            : base(message, ErrorType.Server, statusCode, errorData)
        {
        }
    }

    public class NetworkException : WhatsAppException
    {
        public NetworkException(string message, Exception innerException = null)
            : base(message, ErrorType.Network, 0, null)
        {
        }
    }

    public class FileValidationException : ValidationException
    {
        public enum FileErrorType
        {
            SizeExceeded,
            InvalidFormat,
            InvalidContent,
            EmptyFile
        }

        public FileErrorType FileError { get; }

        public FileValidationException(string message, FileErrorType fileError, JObject errorData = null)
            : base(message, errorData)
        {
            FileError = fileError;
        }
    }
}