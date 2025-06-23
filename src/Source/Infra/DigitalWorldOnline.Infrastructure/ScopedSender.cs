using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalWorldOnline.Infrastructure
{
    public record ScopedSender<TSender>(IServiceProvider Provider) : ISender where TSender : ISender
    {
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct)
        {
            using var scope = Provider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<TSender>();

            return await sender.Send(request, ct);
        }

        public async Task<object?> Send(object request, CancellationToken ct)
        {
            using var scope = Provider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<TSender>();

            return await sender.Send(request, ct);
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request, CancellationToken ct)
        {
            using var scope = Provider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<TSender>();

            return sender.CreateStream(request, ct);
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken ct)
        {
            using var scope = Provider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<TSender>();

            return sender.CreateStream(request, ct);
        }
    }
}