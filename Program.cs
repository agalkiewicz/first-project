using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IMovieService, MovieService>();

builder.Services.AddOptions<WeatherOptions>().BindConfiguration(nameof(WeatherOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex)
        when (ex.Message.Contains("Failed to read parameter", StringComparison.OrdinalIgnoreCase)
              && ex.Message.Contains("from the request body as JSON", StringComparison.OrdinalIgnoreCase))
    {
        var sourceMessage = ex.InnerException?.Message ?? ex.Message;
        var pathMatch = Regex.Match(sourceMessage, @"Path:\s\$\.(?<path>[^\s|]+)");

        var fieldName = "General";
        if (pathMatch.Success)
        {
            var rawPath = pathMatch.Groups["path"].Value;
            var firstSegment = rawPath.Split('.', '[', ']')[0];
            if (!string.IsNullOrWhiteSpace(firstSegment))
            {
                fieldName = char.ToUpperInvariant(firstSegment[0]) + firstSegment[1..];
            }
        }

        var errorMessage = $"The {fieldName} field is invalid.";

        if (sourceMessage.Contains("System.DateTimeOffset", StringComparison.Ordinal))
        {
            errorMessage = $"The {fieldName} field must be a valid date and time.";
        }
        else if (sourceMessage.Contains("System.Double", StringComparison.Ordinal)
                 || sourceMessage.Contains("System.Decimal", StringComparison.Ordinal)
                 || sourceMessage.Contains("System.Int", StringComparison.Ordinal))
        {
            errorMessage = $"The {fieldName} field must be a valid number.";
        }

        var errors = new Dictionary<string, string[]>
        {
            [fieldName] = [errorMessage]
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await Results.ValidationProblem(errors).ExecuteAsync(context);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await using (var serviceScope = app.Services.CreateAsyncScope())
    await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<MovieDbContext>())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}
else
{
    app.UseHttpsRedirection();
}

app.MapMovieEndpoints();

app.Run();
