using System;
using System.Text.RegularExpressions;
using Wapi2.SDK.Exceptions;
using Wapi2.SDK.Models;

namespace Wapi2.SDK.Utils
{
    internal static class FileUtils
    {
        private static readonly Regex Base64Regex = new Regex(@"^data:.*?;base64,", RegexOptions.Compiled);

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            return result && (uriResult.Scheme == "http" || uriResult.Scheme == "https");
        }

        public static bool IsBase64String(string content)
        {
            if (string.IsNullOrEmpty(content))
                return false;

            // Remove data URI scheme if present
            string base64 = Base64Regex.Replace(content, "");

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static long GetBase64FileSize(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return 0;

            string base64 = Base64Regex.Replace(base64String, "");
            int padding = 0;

            if (base64.EndsWith("=="))
                padding = 2;
            else if (base64.EndsWith("="))
                padding = 1;

            return (base64.Length * 3) / 4 - padding;
        }

        public static void ValidateFileContent(string content, string contentType = "File")
        {
            if (string.IsNullOrEmpty(content))
                throw new ValidationException($"{contentType} content cannot be empty");

            if (!IsValidUrl(content) && !IsBase64String(content))
                throw new ValidationException($"Invalid {contentType.ToLower()} content. Must be a valid URL or Base64 string");

            if (IsBase64String(content))
            {
                var fileSize = GetBase64FileSize(content);
                if (fileSize > Constants.MAX_FILE_SIZE)
                    throw new FileValidationException(
                        $"File size exceeds maximum limit of {Constants.MAX_FILE_SIZE / (1024 * 1024)}MB",
                        FileValidationException.FileErrorType.SizeExceeded
                    );
            }
        }
    }
}