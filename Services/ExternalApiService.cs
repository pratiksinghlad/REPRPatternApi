using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPRPatternApi.Constants;
using REPRPatternApi.Models;

namespace REPRPatternApi.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalApiService> _logger;
    private readonly ExternalApiSettings _externalApiSettings;
    private const string _clientName = ApiConstants.ExternalApiClientName;

    public ExternalApiService(IHttpClientFactory httpClientFactory, ILogger<ExternalApiService> logger,
        IOptions<ExternalApiSettings> externalApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _externalApiSettings = externalApiSettings.Value;
    }

    public async Task<T> GetAsync<T>()
    {
        string endpoint = _externalApiSettings.BaseUrl;

        try
        {
            // Get a new HttpClient instance from the factory
            // The factory manages the lifetime of the HttpClient
            var httpClient = _httpClientFactory.CreateClient(_clientName);

            var httpResponse = await httpClient.GetAsync(_externalApiSettings.BaseUrl);

            if (!httpResponse.IsSuccessStatusCode)
            {
                //Logger.LogError("Http GET request (url: {0}) failed with an unsuccessful statuscode (Code: {1}). Response: {2}", url, httpResponse.StatusCode, responseContent);
                return default(T)!;
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent)) return default(T)!;

            return JsonConvert.DeserializeObject<T>(responseContent) ?? default(T)!;
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
}