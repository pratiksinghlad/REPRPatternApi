using REPRPatternApi.Models.Responses;
using REPRPatternApi.Models.Responses.External;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.External;

/// <summary>
/// Endpoint to retrieve external API data
/// </summary>
public sealed class GetExternalDataEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("external/data", HandleAsync)
            .WithName("GetExternalData")
            .WithSummary("Get external API data")
            .WithDescription("Retrieves data from external API with resilience policies")
            .WithTags("External")
            .Produces<ExternalDataCollectionResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to get external API data
    /// </summary>
    /// <param name="externalApiService">External API service</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External data response</returns>
    private static Task<IResult> HandleAsync(
        IExternalApiService externalApiService,
        ILogger<GetExternalDataEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "GetExternalData",
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Retrieving data from external API");

        try
        {
            // This is a mock implementation - in real scenarios, 
            // the external service would call a real external API
            var mockData = new[]
            {
                new ExternalDataResponse(1, "External User 1", "user1"),
                new ExternalDataResponse(2, "External User 2", "user2"),
                new ExternalDataResponse(3, "External User 3", "user3")
            };

            var response = new ExternalDataCollectionResponse(mockData);
            
            logger.LogInformation("Successfully retrieved {DataCount} external data items", mockData.Length);
            return Task.FromResult(Results.Ok(response));
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "External API service unavailable");
            return Task.FromResult(Results.Problem(
                title: "Service Unavailable",
                detail: "The external API service is currently unavailable",
                statusCode: StatusCodes.Status503ServiceUnavailable));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving external data");
            return Task.FromResult(Results.Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving external data",
                statusCode: StatusCodes.Status500InternalServerError));
        }
    }
}