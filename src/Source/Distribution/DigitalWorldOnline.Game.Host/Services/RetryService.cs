using Serilog;

namespace DigitalWorldOnline.Game.Services
{
    public class RetryService
    {
        private readonly ILogger _logger;

        public RetryService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            delay ??= TimeSpan.FromMilliseconds(500);
            shouldRetry ??= DefaultShouldRetry;

            Exception? lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var result = await operation();
                    
                    if (attempt > 1)
                    {
                        _logger.Information($"[RETRY_SUCCESS] Operation '{operationName}' succeeded on attempt {attempt}");
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    if (attempt == maxRetries || !shouldRetry(ex))
                    {
                        _logger.Error(ex, $"[RETRY_FAILED] Operation '{operationName}' failed after {attempt} attempts");
                        throw;
                    }

                    _logger.Warning($"[RETRY_ATTEMPT] Operation '{operationName}' failed on attempt {attempt}, retrying in {delay.Value.TotalMilliseconds}ms. Error: {ex.Message}");
                    
                    await Task.Delay(delay.Value);
                    
                    // Exponential backoff
                    delay = TimeSpan.FromMilliseconds(delay.Value.TotalMilliseconds * 1.5);
                }
            }

            throw lastException ?? new InvalidOperationException($"Operation '{operationName}' failed without exception");
        }

        public async Task ExecuteWithRetryAsync(
            Func<Task> operation,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            await ExecuteWithRetryAsync(async () =>
            {
                await operation();
                return true;
            }, operationName, maxRetries, delay, shouldRetry);
        }

        public T ExecuteWithRetry<T>(
            Func<T> operation,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            delay ??= TimeSpan.FromMilliseconds(500);
            shouldRetry ??= DefaultShouldRetry;

            Exception? lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var result = operation();
                    
                    if (attempt > 1)
                    {
                        _logger.Information($"[RETRY_SUCCESS] Operation '{operationName}' succeeded on attempt {attempt}");
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    if (attempt == maxRetries || !shouldRetry(ex))
                    {
                        _logger.Error(ex, $"[RETRY_FAILED] Operation '{operationName}' failed after {attempt} attempts");
                        throw;
                    }

                    _logger.Warning($"[RETRY_ATTEMPT] Operation '{operationName}' failed on attempt {attempt}, retrying in {delay.Value.TotalMilliseconds}ms. Error: {ex.Message}");
                    
                    Thread.Sleep(delay.Value);
                    
                    // Exponential backoff
                    delay = TimeSpan.FromMilliseconds(delay.Value.TotalMilliseconds * 1.5);
                }
            }

            throw lastException ?? new InvalidOperationException($"Operation '{operationName}' failed without exception");
        }

        public void ExecuteWithRetry(
            Action operation,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            ExecuteWithRetry(() =>
            {
                operation();
                return true;
            }, operationName, maxRetries, delay, shouldRetry);
        }

        private static bool DefaultShouldRetry(Exception ex)
        {
            // Retry para exceções temporárias comuns
            return ex is TimeoutException ||
                   ex is TaskCanceledException ||
                   ex is InvalidOperationException ||
                   (ex is System.Data.Common.DbException dbEx && IsTransientDbError(dbEx)) ||
                   ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                   ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTransientDbError(System.Data.Common.DbException dbEx)
        {
            // Códigos de erro SQL Server que indicam problemas temporários
            var transientErrorNumbers = new[]
            {
                2,      // Timeout
                53,     // Network path not found
                121,    // Semaphore timeout
                1205,   // Deadlock
                1222,   // Lock request timeout
                8645,   // Memory pressure
                8651,   // Low memory condition
                40197,  // Service error
                40501,  // Service busy
                40613,  // Database unavailable
                49918,  // Cannot process request
                49919,  // Cannot process create or update request
                49920   // Cannot process request
            };

            return transientErrorNumbers.Contains(dbEx.ErrorCode);
        }
    }

    public static class RetryExtensions
    {
        public static async Task<T> WithRetryAsync<T>(
            this Task<T> task,
            RetryService retryService,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            return await retryService.ExecuteWithRetryAsync(() => task, operationName, maxRetries, delay, shouldRetry);
        }

        public static async Task WithRetryAsync(
            this Task task,
            RetryService retryService,
            string operationName,
            int maxRetries = 3,
            TimeSpan? delay = null,
            Func<Exception, bool>? shouldRetry = null)
        {
            await retryService.ExecuteWithRetryAsync(() => task, operationName, maxRetries, delay, shouldRetry);
        }
    }
}
