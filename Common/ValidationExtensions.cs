using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Movies.Api.Common;

public static class ValidationExtensions
{
    public static IResult? ValidateRequest<T>(this T? model)
    {
        if (model is null)
        {
            var nullBodyErrors = new Dictionary<string, string[]>
            {
                ["General"] = ["Request body is required."]
            };

            return Results.ValidationProblem(nullBodyErrors);
        }

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model);

        if (Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true))
        {
            return null;
        }

        var errors = validationResults
            .SelectMany(
                result => result.MemberNames.DefaultIfEmpty("General"),
                (result, memberName) => new { MemberName = memberName, result.ErrorMessage })
            .Where(x => x.ErrorMessage is not null)
            .GroupBy(x => x.MemberName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage!).ToArray());

        return Results.ValidationProblem(errors);
    }
}

