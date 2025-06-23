using DigitalWorldOnline.Admin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace DigitalWorldOnline.Admin.Middleware
{
    public class SecurityAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityAuditMiddleware> _logger;

        public SecurityAuditMiddleware(RequestDelegate next, ILogger<SecurityAuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            var originalStatusCode = context.Response.StatusCode;
            
            await _next(context);

            // Log unauthorized access attempts
            if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            {
                var ipAddress = context.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                var path = context.Request.Path.ToString();

                await auditService.LogUnauthorizedAccessAsync(path, ipAddress, userAgent);
                
                _logger.LogWarning("Unauthorized access attempt: {Path} from {IpAddress}", path, ipAddress);
            }

            // Log suspicious activities
            if (IsSuspiciousRequest(context))
            {
                var ipAddress = context.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                var path = context.Request.Path.ToString();

                await auditService.LogUnauthorizedAccessAsync($"SUSPICIOUS: {path}", ipAddress, userAgent);
                
                _logger.LogWarning("Suspicious request detected: {Path} from {IpAddress}", path, ipAddress);
            }
        }

        private bool IsSuspiciousRequest(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLowerInvariant();
            var query = context.Request.QueryString.ToString().ToLowerInvariant();

            // Check for common attack patterns
            var suspiciousPatterns = new[]
            {
                "../", "..\\", "..", 
                "script", "javascript:", "vbscript:",
                "union", "select", "insert", "delete", "drop",
                "exec", "execute", "sp_",
                "<script", "</script>", "alert(", "confirm(",
                "document.cookie", "document.location",
                "eval(", "expression(", "onload=", "onerror="
            };

            return suspiciousPatterns.Any(pattern => 
                path.Contains(pattern) || query.Contains(pattern));
        }
    }

    public static class SecurityAuditMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityAudit(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityAuditMiddleware>();
        }
    }
}
