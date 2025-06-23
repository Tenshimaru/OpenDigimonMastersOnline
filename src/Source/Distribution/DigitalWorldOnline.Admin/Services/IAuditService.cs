using DigitalWorldOnline.Admin.Models;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Services
{
    public interface IAuditService
    {
        Task LogSecurityEventAsync(SecurityEvent securityEvent);
        Task LogLoginAttemptAsync(string username, bool success, string ipAddress, string userAgent);
        Task LogDownloadAttemptAsync(string username, string fileName, bool success, string ipAddress);
        Task LogAdminActionAsync(string username, string action, string details, string ipAddress);
        Task LogUnauthorizedAccessAsync(string path, string ipAddress, string userAgent);
    }
}
