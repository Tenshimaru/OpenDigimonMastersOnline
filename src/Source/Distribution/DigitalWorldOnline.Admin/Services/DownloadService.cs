using DigitalWorldOnline.Admin.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace DigitalWorldOnline.Admin.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuditService _auditService;

        public DownloadService(ILogger<DownloadService> logger, IWebHostEnvironment environment, IAuditService auditService)
        {
            _logger = logger;
            _environment = environment;
            _auditService = auditService;
        }

        public async Task<DownloadResult> GetDownloadAsync(string fileName, string architecture)
        {
            try
            {
                if (!IsValidDownloadRequest(fileName, architecture))
                {
                    _logger.LogWarning("Invalid download request: {FileName}, {Architecture}", fileName, architecture);
                    return new DownloadResult 
                    { 
                        Success = false, 
                        ErrorMessage = "Invalid download request" 
                    };
                }

                var filePath = GetSecureFilePath(fileName, architecture);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Download file not found: {FilePath}", filePath);
                    return new DownloadResult 
                    { 
                        Success = false, 
                        ErrorMessage = "File not found" 
                    };
                }

                var fileInfo = new FileInfo(filePath);
                
                if (fileInfo.Length > DownloadConstants.MaxFileSize)
                {
                    _logger.LogWarning("File too large: {FilePath}, Size: {Size}", filePath, fileInfo.Length);
                    return new DownloadResult 
                    { 
                        Success = false, 
                        ErrorMessage = "File too large" 
                    };
                }

                var fileData = await File.ReadAllBytesAsync(filePath);
                var mimeType = GetMimeType(fileName);

                _logger.LogInformation("Download successful: {FileName}, Size: {Size}", fileName, fileData.Length);

                return new DownloadResult
                {
                    Success = true,
                    FileData = fileData,
                    FileName = fileName,
                    MimeType = mimeType,
                    FileSize = fileData.Length
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
                return new DownloadResult 
                { 
                    Success = false, 
                    ErrorMessage = "Internal server error" 
                };
            }
        }

        public bool IsValidDownloadRequest(string fileName, string architecture)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(architecture))
                return false;

            // Validate architecture
            if (!DownloadConstants.AllowedArchitectures.Contains(architecture.ToLowerInvariant()))
                return false;

            // Validate file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!DownloadConstants.AllowedExtensions.Contains(extension))
                return false;

            // Prevent path traversal
            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return false;

            // Validate filename pattern (only alphanumeric, dots, dashes, underscores)
            if (!System.Text.RegularExpressions.Regex.IsMatch(fileName, @"^[a-zA-Z0-9._-]+$"))
                return false;

            return true;
        }

        public IEnumerable<DownloadInfo> GetAvailableDownloads()
        {
            var downloads = new List<DownloadInfo>();
            var downloadsPath = Path.Combine(_environment.WebRootPath, "Downloads");

            if (!Directory.Exists(downloadsPath))
            {
                _logger.LogWarning("Downloads directory not found: {Path}", downloadsPath);
                return downloads;
            }

            foreach (var architecture in DownloadConstants.AllowedArchitectures)
            {
                var archPath = Path.Combine(downloadsPath, architecture);
                if (!Directory.Exists(archPath))
                    continue;

                var files = Directory.GetFiles(archPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => DownloadConstants.AllowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    downloads.Add(new DownloadInfo
                    {
                        FileName = fileInfo.Name,
                        DisplayName = GetDisplayName(fileInfo.Name, architecture),
                        Architecture = architecture,
                        Description = GetFileDescription(fileInfo.Name),
                        FileSize = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime,
                        IsAvailable = true
                    });
                }
            }

            return downloads;
        }

        private string GetSecureFilePath(string fileName, string architecture)
        {
            var sanitizedFileName = Path.GetFileName(fileName); // Remove any path components
            return Path.Combine(_environment.WebRootPath, "Downloads", architecture, sanitizedFileName);
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return DownloadConstants.MimeTypes.TryGetValue(extension, out var mimeType) 
                ? mimeType 
                : DownloadConstants.DefaultMimeType;
        }

        private string GetDisplayName(string fileName, string architecture)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            return $"{nameWithoutExtension} ({architecture.ToUpperInvariant()})";
        }

        private string GetFileDescription(string fileName)
        {
            if (fileName.ToLowerInvariant().Contains("installer"))
                return "Game Client Installer";
            if (fileName.ToLowerInvariant().Contains("update"))
                return "Game Update Package";
            return "Game File";
        }
    }
}
