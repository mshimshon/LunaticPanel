using LuncaticPanel.Package.Server.Application.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Commands;
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
            case CreateManifestCommand c:
                return ServiceProvider.GetRequiredService<IRequestHandler<CreateManifestCommand>>().HandleAsync(c, ct);
            case HideManifestVersionCommand c:
                return ServiceProvider.GetRequiredService<IRequestHandler<HideManifestVersionCommand>>().HandleAsync(c, ct);
            case EndManifestLifeCommand c:
                return ServiceProvider.GetRequiredService<IRequestHandler<EndManifestLifeCommand>>().HandleAsync(c, ct);
            default:
                throw new MediatorCommandNotFoundException();
        }
    }

    public Task<TResult> ExecuteAsync<TResult>(IRequest request, CancellationToken ct = default)
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
            case GetAllPackageVersionsQuery q:
                return ServiceProvider.GetRequiredService<IRequestHandler<GetAllPackageVersionsQuery, TResult>>().HandleAsync(q, ct);
            case GetSpecificPackageVersionQuery q:
                return ServiceProvider.GetRequiredService<IRequestHandler<GetSpecificPackageVersionQuery, TResult>>().HandleAsync(q, ct);
            case GetLatestPackageQuery q:
                return ServiceProvider.GetRequiredService<IRequestHandler<GetLatestPackageQuery, TResult>>().HandleAsync(q, ct);
            case SearchManifestQuery q:
                return ServiceProvider.GetRequiredService<IRequestHandler<SearchManifestQuery, TResult>>().HandleAsync(q, ct);
            default:
                throw new MediatorCommandNotFoundException();
        }
    }
}
