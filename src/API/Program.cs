using API.Extensions;
using Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//JWT
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                         Encoding.UTF8.GetBytes(
                                             builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero // token expira no tempo exato
        };
    });

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
                .AddInfraServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.UseScalarDocumentation();

await app.InitializeDb();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapEndpoints();

app.MapGet("/", () => Results.Ok("TechChallenge API - Running"));

app.Run();

public partial class Program() { }
