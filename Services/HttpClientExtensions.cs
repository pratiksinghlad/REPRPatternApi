using System.Net;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using REPRPatternApi.Constants;
using REPRPatternApi.Services;

namespace REPRPatternApi.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddExternalApiHttpClient(this IServiceCollection services, string baseApiUrl)
    {
        services.AddHttpClient(ApiConstants.ExternalApiClientName, client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy())
            .AddPolicyHandler(GetTimeoutPolicy());

        services.AddScoped<IExternalApiService, ExternalApiService>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
                {
                    var serviceProvider = context.GetServiceProvider();
                    var logger = serviceProvider?.GetService<ILogger<HttpClient>>();
                    logger?.LogWarning(
                        exception.Exception,
                        "Error {ExceptionMessage} on attempt {AttemptNumber}. Waiting {SleepDuration} before next retry.",
                        exception.Exception?.Message,
                        attemptNumber,
                        sleepDuration);
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration, context) =>
                {
                    var serviceProvider = context.GetServiceProvider();
                    var logger = serviceProvider?.GetService<ILogger<HttpClient>>();
                    logger?.LogWarning(
                        "Circuit breaker opened for {Duration} due to: {ExceptionMessage}",
                        duration,
                        exception.Exception?.Message);
                },
                onReset: context =>
                {
                    var serviceProvider = context.GetServiceProvider();
                    var logger = serviceProvider?.GetService<ILogger<HttpClient>>();
                    logger?.LogInformation("Circuit breaker reset");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
    }

    private static IServiceProvider? GetServiceProvider(this Context context)
    {
        return context.TryGetValue("ServiceProvider", out var serviceProvider) 
            ? serviceProvider as IServiceProvider 
            : null;
    }
}