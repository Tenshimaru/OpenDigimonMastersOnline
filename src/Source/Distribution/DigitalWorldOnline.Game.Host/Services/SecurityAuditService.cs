using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Account;
using Serilog;
using System.Collections.Concurrent;

namespace DigitalWorldOnline.Game.Services
{
    public class SecurityAuditService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<long, SecurityTracker> _playerTrackers = new();

        public SecurityAuditService(ILogger logger)
        {
            _logger = logger;
        }

        public void LogTradeAttempt(GameClient client, GameClient? targetClient, string action, bool success, string details = "")
        {
            var logEntry = new SecurityLogEntry
            {
                Timestamp = DateTime.Now,
                PlayerId = client.TamerId,
                PlayerName = client.Tamer.Name,
                Action = $"TRADE_{action}",
                Success = success,
                Details = details,
                TargetPlayerId = targetClient?.TamerId,
                TargetPlayerName = targetClient?.Tamer.Name,
                IpAddress = client.ClientAddress
            };

            LogSecurityEvent(logEntry);
            UpdatePlayerTracker(client.TamerId, action, success);
        }

        public void LogHatchAttempt(GameClient client, string action, bool success, string details = "")
        {
            var logEntry = new SecurityLogEntry
            {
                Timestamp = DateTime.Now,
                PlayerId = client.TamerId,
                PlayerName = client.Tamer.Name,
                Action = $"HATCH_{action}",
                Success = success,
                Details = details,
                IpAddress = client.ClientAddress
            };

            LogSecurityEvent(logEntry);
            UpdatePlayerTracker(client.TamerId, action, success);
        }

        public void LogInventoryOperation(GameClient client, string action, bool success, string itemDetails = "")
        {
            var logEntry = new SecurityLogEntry
            {
                Timestamp = DateTime.Now,
                PlayerId = client.TamerId,
                PlayerName = client.Tamer.Name,
                Action = $"INVENTORY_{action}",
                Success = success,
                Details = itemDetails,
                IpAddress = client.ClientAddress
            };

            LogSecurityEvent(logEntry);
            UpdatePlayerTracker(client.TamerId, action, success);
        }

        public bool IsSuspiciousActivity(long playerId, string action)
        {
            if (!_playerTrackers.TryGetValue(playerId, out var tracker))
                return false;

            var recentFailures = tracker.GetRecentFailures(action, TimeSpan.FromMinutes(5));
            var recentAttempts = tracker.GetRecentAttempts(action, TimeSpan.FromMinutes(1));

            // Mais de 3 falhas em 5 minutos ou mais de 10 tentativas em 1 minuto
            return recentFailures >= 3 || recentAttempts >= 10;
        }

        public void LogSuspiciousActivity(GameClient client, string reason, string details = "")
        {
            var logEntry = new SecurityLogEntry
            {
                Timestamp = DateTime.Now,
                PlayerId = client.TamerId,
                PlayerName = client.Tamer.Name,
                Action = "SUSPICIOUS_ACTIVITY",
                Success = false,
                Details = $"{reason}: {details}",
                IpAddress = client.ClientAddress,
                Severity = SecuritySeverity.High
            };

            LogSecurityEvent(logEntry);
            
            _logger.Warning($"[SECURITY_ALERT] Suspicious activity detected for {client.Tamer.Name}: {reason} - {details}");
        }

        private void LogSecurityEvent(SecurityLogEntry entry)
        {
            var logLevel = entry.Severity switch
            {
                SecuritySeverity.Low => Serilog.Events.LogEventLevel.Information,
                SecuritySeverity.Medium => Serilog.Events.LogEventLevel.Warning,
                SecuritySeverity.High => Serilog.Events.LogEventLevel.Error,
                _ => Serilog.Events.LogEventLevel.Information
            };

            _logger.Write(logLevel, 
                "[SECURITY_AUDIT] Player: {PlayerName} ({PlayerId}) | Action: {Action} | Success: {Success} | Details: {Details} | IP: {IpAddress} | Target: {TargetPlayerName}",
                entry.PlayerName, entry.PlayerId, entry.Action, entry.Success, entry.Details, entry.IpAddress, entry.TargetPlayerName);
        }

        private void UpdatePlayerTracker(long playerId, string action, bool success)
        {
            var tracker = _playerTrackers.GetOrAdd(playerId, _ => new SecurityTracker());
            tracker.RecordAttempt(action, success);

            // Limpar trackers antigos periodicamente
            if (_playerTrackers.Count > 10000)
            {
                CleanupOldTrackers();
            }
        }

        private void CleanupOldTrackers()
        {
            var cutoffTime = DateTime.Now.AddHours(-1);
            var keysToRemove = new List<long>();

            foreach (var kvp in _playerTrackers)
            {
                if (kvp.Value.LastActivity < cutoffTime)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove.Take(1000)) // Limitar para não travar
            {
                _playerTrackers.TryRemove(key, out _);
            }
        }
    }

    public class SecurityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public long PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Details { get; set; } = string.Empty;
        public long? TargetPlayerId { get; set; }
        public string? TargetPlayerName { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public SecuritySeverity Severity { get; set; } = SecuritySeverity.Low;
    }

    public enum SecuritySeverity
    {
        Low,
        Medium,
        High
    }

    public class SecurityTracker
    {
        private readonly ConcurrentDictionary<string, List<SecurityAttempt>> _attempts = new();
        public DateTime LastActivity { get; private set; } = DateTime.Now;

        public void RecordAttempt(string action, bool success)
        {
            LastActivity = DateTime.Now;
            var attempts = _attempts.GetOrAdd(action, _ => new List<SecurityAttempt>());
            
            lock (attempts)
            {
                attempts.Add(new SecurityAttempt { Timestamp = DateTime.Now, Success = success });
                
                // Manter apenas os últimos 100 registros por ação
                if (attempts.Count > 100)
                {
                    attempts.RemoveRange(0, attempts.Count - 100);
                }
            }
        }

        public int GetRecentFailures(string action, TimeSpan timeSpan)
        {
            if (!_attempts.TryGetValue(action, out var attempts))
                return 0;

            var cutoffTime = DateTime.Now - timeSpan;
            lock (attempts)
            {
                return attempts.Count(a => !a.Success && a.Timestamp >= cutoffTime);
            }
        }

        public int GetRecentAttempts(string action, TimeSpan timeSpan)
        {
            if (!_attempts.TryGetValue(action, out var attempts))
                return 0;

            var cutoffTime = DateTime.Now - timeSpan;
            lock (attempts)
            {
                return attempts.Count(a => a.Timestamp >= cutoffTime);
            }
        }
    }

    public class SecurityAttempt
    {
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
    }
}
