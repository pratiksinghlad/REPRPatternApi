using FluentValidation;
using MediatR;

namespace REPRPatternApi.Application.Behaviors;

/// <summary>
/// Pipeline behavior for FluentValidation
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the ValidationPipelineBehavior
    /// </summary>
    /// <param name="validators">Collection of validators</param>
    /// <param name="logger">Logger instance</param>
    public ValidationPipelineBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request and performs validation
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">Next handler in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            _logger.LogInformation("Validating request {RequestType}", typeof(TRequest).Name);
            
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                _logger.LogWarning("Validation failed for {RequestType}: {Errors}", 
                    typeof(TRequest).Name, 
                    string.Join(", ", failures.Select(f => f.ErrorMessage)));
                
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}