using System.Net;
using Polly;
using Polly.Extensions.Http;
using REPRPatternApi.Constants;
using REPRPatternApi.Services;

namespace REPRPatternApi.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddExternalApiHttpClient(this IServiceCollection services, string baseApiUrl)
    {
        // Register the named HttpClient with policies
        services.AddHttpClient(ApiConstants.ExternalApiClientName, client =>
            {
                client.BaseAddress = new Uri(baseApiUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(GetRetryPolicy());
        
        services.AddScoped<IExternalApiService, ExternalApiService>(); // <--- Add the service to the DI container>

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests || msg.StatusCode == HttpStatusCode.Forbidden) // 429 Too Many Requests
            .WaitAndRetryAsync(
                retryCount: 3, // Number of retries
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2, 4, 8 seconds
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} encountered an error: {outcome.Exception?.Message}. Waiting {timespan} before next retry.");
                });
    }
}