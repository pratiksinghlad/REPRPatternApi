using System.ComponentModel.DataAnnotations;

namespace REPRPatternApi.Extensions;

/// <summary>
/// Extension methods for request validation
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates a request model and returns validation results
    /// </summary>
    /// <typeparam name="T">Type of the request model</typeparam>
    /// <param name="request">The request to validate</param>
    /// <returns>Validation results</returns>
    public static IResult? ValidateRequest<T>(T request) where T : class
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = validationResults
                .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ErrorMessage ?? "Invalid value").ToArray()
                );

            return Results.ValidationProblem(errors);
        }

        return null;
    }
}