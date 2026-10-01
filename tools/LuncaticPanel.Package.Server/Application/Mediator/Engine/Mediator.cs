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

    public async Task ExecuteAsync(IRequest request, CancellationToken ct = default)
    {
        Console.WriteLine($"Mediator Pipeline (No Result) Request {request.GetType()}");
        switch (request)
        {

            case HideManifestVersionCommand c:
                await ServiceProvider.GetRequiredService<IRequestHandler<HideManifestVersionCommand>>().HandleAsync(c, ct);
                return;
            case EndManifestLifeCommand c:
                await ServiceProvider.GetRequiredService<IRequestHandler<EndManifestLifeCommand>>().HandleAsync(c, ct);
                return;
            default:
                throw new MediatorCommandNotFoundException();
        }
    }



    public async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken ct = default)
    {
        Console.WriteLine($"Mediator Pipline Request {request.GetType()}");
        switch (request)
        {
            case CreateManifestCommand c:
                return await ServiceProvider.GetRequiredService<IRequestHandler<CreateManifestCommand, TResult>>().HandleAsync(c, ct);
            case GetAllPackageVersionsQuery q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<GetAllPackageVersionsQuery, TResult>>().HandleAsync(q, ct);
            case GetSpecificPackageVersionQuery q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<GetSpecificPackageVersionQuery, TResult>>().HandleAsync(q, ct);
            case GetLatestPackageQuery q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<GetLatestPackageQuery, TResult>>().HandleAsync(q, ct);
            case SearchManifestQuery q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<SearchManifestQuery, TResult>>().HandleAsync(q, ct);
            case PackageValidationCommand q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<PackageValidationCommand, TResult>>().HandleAsync(q, ct);
            case GetPackageDownloadTargetQuery q:
                return await ServiceProvider.GetRequiredService<IRequestHandler<GetPackageDownloadTargetQuery, TResult>>().HandleAsync(q, ct);
            default:
                throw new MediatorCommandNotFoundException();
        }
    }
}
