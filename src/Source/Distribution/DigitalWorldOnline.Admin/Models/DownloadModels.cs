using System;
using System.Collections.Generic;

namespace DigitalWorldOnline.Admin.Models
{
    public class DownloadResult
    {
        public bool Success { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
        public string? MimeType { get; set; }
        public string? ErrorMessage { get; set; }
        public long FileSize { get; set; }
    }

    public class DownloadInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Architecture { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class SecurityEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public static class SecurityEventTypes
    {
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string Download = "DOWNLOAD";
        public const string AdminAction = "ADMIN_ACTION";
        public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
        public const string SecurityViolation = "SECURITY_VIOLATION";
    }

    public static class DownloadConstants
    {
        public static readonly string[] AllowedArchitectures = { "x64", "x86" };
        public static readonly string[] AllowedExtensions = { ".zip", ".exe", ".msi" };
        public static readonly string DownloadsBasePath = "wwwroot/Downloads";
        public static readonly long MaxFileSize = 500 * 1024 * 1024; // 500MB
        public static readonly string DefaultMimeType = "application/octet-stream";

        public static readonly Dictionary<string, string> MimeTypes = new()
        {
            { ".zip", "application/zip" },
            { ".exe", "application/x-msdownload" },
            { ".msi", "application/x-msi" }
        };
    }
}
