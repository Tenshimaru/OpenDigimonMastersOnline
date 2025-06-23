using DigitalWorldOnline.Admin.Models;
using System.Text.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace DigitalWorldOnline.Admin.Services
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;
        private readonly IWebHostEnvironment _environment;

        public AuditService(ILogger<AuditService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task LogSecurityEventAsync(SecurityEvent securityEvent)
        {
            try
            {
                // Log to structured logger
                _logger.LogInformation("Security Event: {EventType} | User: {Username} | IP: {IpAddress} | Success: {Success} | Details: {Details}",
                    securityEvent.EventType,
                    securityEvent.Username,
                    securityEvent.IpAddress,
                    securityEvent.Success,
                    securityEvent.Details);

                // Also log to dedicated security log file
                await LogToSecurityFileAsync(securityEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log security event");
            }
        }

        public async Task LogLoginAttemptAsync(string username, bool success, string ipAddress, string userAgent)
        {
            var securityEvent = new SecurityEvent
            {
                EventType = SecurityEventTypes.Login,
                Username = username,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Action = "LOGIN_ATTEMPT",
                Success = success,
                Details = success ? "Login successful" : "Login failed"
            };

            await LogSecurityEventAsync(securityEvent);
        }

        public async Task LogDownloadAttemptAsync(string username, string fileName, bool success, string ipAddress)
        {
            var securityEvent = new SecurityEvent
            {
                EventType = SecurityEventTypes.Download,
                Username = username,
                IpAddress = ipAddress,
                Action = "DOWNLOAD_ATTEMPT",
                Success = success,
                Details = $"File: {fileName}"
            };

            await LogSecurityEventAsync(securityEvent);
        }

        public async Task LogAdminActionAsync(string username, string action, string details, string ipAddress)
        {
            var securityEvent = new SecurityEvent
            {
                EventType = SecurityEventTypes.AdminAction,
                Username = username,
                IpAddress = ipAddress,
                Action = action,
                Success = true,
                Details = details
            };

            await LogSecurityEventAsync(securityEvent);
        }

        public async Task LogUnauthorizedAccessAsync(string path, string ipAddress, string userAgent)
        {
            var securityEvent = new SecurityEvent
            {
                EventType = SecurityEventTypes.UnauthorizedAccess,
                Username = "ANONYMOUS",
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Action = "UNAUTHORIZED_ACCESS",
                Success = false,
                Details = $"Attempted to access: {path}"
            };

            await LogSecurityEventAsync(securityEvent);
        }

        private async Task LogToSecurityFileAsync(SecurityEvent securityEvent)
        {
            try
            {
                var logsDirectory = Path.Combine(_environment.ContentRootPath, "logs", "Security");
                Directory.CreateDirectory(logsDirectory);

                var fileName = $"security-{DateTime.UtcNow:yyyy-MM-dd}.log";
                var filePath = Path.Combine(logsDirectory, fileName);

                var logEntry = JsonSerializer.Serialize(securityEvent, new JsonSerializerOptions
                {
                    WriteIndented = false
                });

                var logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC | {logEntry}{Environment.NewLine}";

                await File.AppendAllTextAsync(filePath, logLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to security log file");
            }
        }
    }
}
