// Models/Session.cs
using System;

namespace Wapi2.SDK.Models
{
    public class Session
    {
        public string SessionId { get; set; }
        public SessionStatus Status { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum SessionStatus
    {
        Initializing,
        QrReady,
        Authenticated,
        Ready,
        Disconnected,
        Failed
    }
}

// Models/Contact.cs
namespace Wapi2.SDK.Models
{
    public class Contact
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
    }
}

// Models/Group.cs
namespace Wapi2.SDK.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanPost { get; set; }
    }
}

// Models/ApiResponse.cs
namespace Wapi2.SDK.Models
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}

// Models/Constants.cs
namespace Wapi2.SDK.Models
{
    public static class Constants
    {
        public const int MAX_FILENAME_LENGTH = 255;
        public const int MAX_CAPTION_LENGTH = 1024;
        public const int MAX_FILE_SIZE = 16 * 1024 * 1024; // 16MB
        public static readonly string[] ALLOWED_VIDEO_FORMATS = { "mp4", "3gp", "mov" };
        public static readonly string[] ALLOWED_OFFICE_FORMATS = { "doc", "docx", "xls", "xlsx", "ppt", "pptx" };
    }
}