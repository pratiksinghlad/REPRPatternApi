using Newtonsoft.Json;

namespace REPRPatternApi.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalApiService> _logger;
    private const string _clientName = "ExternalApi";

    public ExternalApiService(IHttpClientFactory httpClientFactory, ILogger<ExternalApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            try
            {
                // Get a new HttpClient instance from the factory
                // The factory manages the lifetime of the HttpClient
                var httpClient = _httpClientFactory.CreateClient(_clientName);

                var httpResponse = await httpClient.GetAsync(endpoint);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    //Logger.LogError("Http GET request (url: {0}) failed with an unsuccessful statuscode (Code: {1}). Response: {2}", url, httpResponse.StatusCode, responseContent);
                    return default;
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent)) return default;

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                // Logger.LogError($"An exception ocurred while fetching data in HttpClient: {ex.Message}", ex);
                throw new Exception(
                    JsonConvert.SerializeObject(new
                    {
                        Url = endpoint, Message = "An exception ocurred while fetching data."
                    }), ex);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling external API at {Endpoint}", endpoint);
            throw;
        }
    }
}