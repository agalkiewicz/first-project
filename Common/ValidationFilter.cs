using Microsoft.AspNetCore.Http;

namespace Movies.Api.Common;

public class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.FirstOrDefault(a => a is TRequest) as TRequest;

        var validationResult = argument.ValidateRequest();
        if (validationResult is not null)
        {
            return validationResult;
        }

        return await next(context);
    }
}

