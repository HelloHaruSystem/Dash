using System.ComponentModel.DataAnnotations;

namespace Dash.Api.Common;

internal sealed class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
    {
        // Get the argument to validate of the specified type
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is not null)
        {
            // collect all validation errors, not just the first one
            var validationResults = new List<ValidationResult>();
            // create context describing object being validated, services, metadata
            var validationContext = new ValidationContext(argument);

            // Trigger the DataAnnotations validation engine
            bool isValid = Validator.TryValidateObject(
                    argument,                           // The DTO instance
                    validationContext,                  // Context info for the validator
                    validationResults,                  // validation errors
                    validateAllProperties: true);      // Ensures all properties are validated

            // If any attribute failed execution stops
            if (!isValid)
            {
                // Format the errors into response
                var errors = validationResults
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                    .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage ?? "Invalid value").ToArray()
                    );

                // Return a problem result
                return Results.ValidationProblem(errors);
            }
        }

        return await next(context);
    }
}
