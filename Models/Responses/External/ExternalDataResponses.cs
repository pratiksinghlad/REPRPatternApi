namespace REPRPatternApi.Models.Responses.External;

/// <summary>
/// Response model for external API data
/// </summary>
public record ExternalDataResponse(
    int Id,
    string Name,
    string Login);

/// <summary>
/// Collection response for external API data
/// </summary>
public record ExternalDataCollectionResponse(IEnumerable<ExternalDataResponse> Data);