namespace LuncaticPanel.Package.Server.Application.Mediator.Engine;


public interface IRequestHandler<TCommand>
{
    Task HandleAsync(TCommand data, CancellationToken ct = default);
}

public interface IRequestHandler<TCommand, TRequest>
{
    Task<TRequest> HandleAsync(TCommand data, CancellationToken ct = default);
}
