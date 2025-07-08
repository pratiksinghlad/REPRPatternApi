using FluentValidation;
using REPRPatternApi.Controllers;

namespace REPRPatternApi.Application.Validators;

/// <summary>
/// Validator for GetWeatherForecastQuery
/// </summary>
public class GetWeatherForecastQueryValidator : AbstractValidator<GetWeatherForecastQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetWeatherForecastQueryValidator
    /// </summary>
    public GetWeatherForecastQueryValidator()
    {
        RuleFor(x => x.Days)
            .GreaterThan(0)
            .WithMessage("Days must be greater than 0")
            .LessThanOrEqualTo(30)
            .WithMessage("Days cannot exceed 30");
    }
}

/// <summary>
/// Validator for GetWeatherForecastByDateQuery
/// </summary>
public class GetWeatherForecastByDateQueryValidator : AbstractValidator<GetWeatherForecastByDateQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetWeatherForecastByDateQueryValidator
    /// </summary>
    public GetWeatherForecastByDateQueryValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)))
            .WithMessage("Date cannot be more than 30 days in the past")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now.AddDays(365)))
            .WithMessage("Date cannot be more than 365 days in the future");
    }
}