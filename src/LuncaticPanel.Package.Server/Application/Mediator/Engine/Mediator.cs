using LuncaticPanel.Package.Server.Application.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Application.Mediator.Engine;

internal class Mediator : IMediator
{
    public Mediator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public Task ExecuteAsync(IRequest request, CancellationToken ct = default)
    {
        switch (request)
        {
            default:
                throw new MediatorCommandNotFoundException();
        }
    }
    public Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken ct = default)
    {
        switch (request)
        {
            case SearchManifestQuery q:
                return ServiceProvider.GetRequiredService<IRequestHandler<SearchManifestQuery, TResult>>().HandleAsync(q, ct);
            default:
                throw new MediatorCommandNotFoundException();
        }
    }
}
