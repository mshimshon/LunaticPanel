namespace LuncaticPanel.Package.Server.Application.Mediator.Engine;

public interface IMediator
{
    Task ExecuteAsync(IRequest request, CancellationToken ct = default);
    Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken ct = default);
}
