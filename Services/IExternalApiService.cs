namespace REPRPatternApi.Services;

public interface IExternalApiService
{
    Task<T> GetAsync<T>();
}