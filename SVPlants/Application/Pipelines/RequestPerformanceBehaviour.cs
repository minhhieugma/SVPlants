namespace Application.Pipelines;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger _logger;

    public RequestPerformanceBehaviour(ILoggerFactory loggerFactory)
    {
        _timer = new Stopwatch();

        _logger = loggerFactory.CreateLogger<RequestPerformanceBehaviour<TRequest, TResponse>>();
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        if (_timer.ElapsedMilliseconds > 2000)
        {
            var name = typeof(TRequest).Name;

            string message = $"Long Running Request: {name} ({_timer.ElapsedMilliseconds} milliseconds) {request}";
            _logger.LogWarning(message);
        }

        return response;
    }
}