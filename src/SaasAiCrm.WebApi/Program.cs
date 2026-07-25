using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SaasAiCrm.Application;
using SaasAiCrm.Application.Authentication.Login;
using SaasAiCrm.Infrastructure;
using SaasAiCrm.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);
var jwt = builder.Configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", async (
    LoginRequest request,
    LoginHandler handler,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["credentials"] = ["E-posta ve parola zorunludur."]
        });
    }

    var response = await handler.HandleAsync(
        new LoginCommand(request.Email, request.Password),
        cancellationToken);

    return response is null
        ? Results.Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Giriş başarısız",
            detail: "E-posta veya parola hatalı.")
        : Results.Ok(response);
}).AllowAnonymous();

app.MapGet("/api/auth/me", (System.Security.Claims.ClaimsPrincipal user) => Results.Ok(new
{
    id = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value,
    email = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value,
    fullName = user.Identity?.Name,
    role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
})).RequireAuthorization();

app.MapFallbackToFile("index.html");
app.Run();

public sealed record LoginRequest(string Email, string Password);

public partial class Program;
