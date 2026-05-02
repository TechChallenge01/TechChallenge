using API.Extensions;
using Application;
using Infra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi()
                .AddApplicationServices()
                .AddInfraServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.UseScalarDocumentation();

await app.InitializeDb();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
