using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddOptions<WeatherOptions>().BindConfiguration(nameof(WeatherOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();

//Configuration from AppSettings
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
//User Manager Service
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<MovieDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
//Adding Athentication - JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key configuration is missing")))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        //Seed Default Users
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await MovieDbContext.SeedEssentialsAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

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
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapMovieEndpoints();
app.MapAuthEndpoints();
app.MapCategoryEndpoints();

app.Run();
